// ———————————————————————————————
// <copyright file="RootDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the root dialog where all conversations start or ends.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the root dialog where all conversations start or ends.
    /// </summary>
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string CommandMatchHelp = "help";

        private readonly Uri licenseUri;

        [NonSerialized]
        private TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="licenseUri">Uri to the license.</param>
        /// <param name="telemetryClient">A <see cref="telemetryClient"/>.</param>
        public RootDialog(Uri licenseUri, TelemetryClient telemetryClient)
        {
            this.licenseUri = licenseUri ?? throw new ArgumentNullException(nameof(licenseUri));
            this.telemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));
        }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.ThrowIfNull(nameof(context));

            context.Wait(this.HandleActivityAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Handles any incoming result.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">a <see cref="IMessageActivity"/>.</param>
        /// <returns>a <see cref="Task"/>.</returns>
        public virtual async Task HandleActivityAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            // Occurs when the conversation starts.
            if (string.Equals(activity.Type, ActivityTypes.ConversationUpdate, StringComparison.OrdinalIgnoreCase))
            {
                await this.WelcomeAsync(context, activity);
            }
            else
            {
                await this.HandleCommandAsync(context, activity);
            }
        }

        /// <summary>
        /// Handles any incoming command and route them to the appropriate dialog.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task HandleCommandAsync(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            // Message is coming from a group chat, so skipping if the bot is not explicitly mentioned.
            if (result.Conversation.IsGroup.GetValueOrDefault() && !result.MentionsRecipient())
            {
                context.Wait(this.HandleActivityAsync);
                return;
            }

            var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find(result.RemoveRecipientMention());
            context.UserData.TryGetValue("userData", out UserData data);

            if (dialog == null || data == null)
            {
                if (data == null)
                {
                    data = new UserData();
                }

                var reply = context.MakeMessage();

                reply.Text = result.Text.Trim().Equals(CommandMatchHelp, StringComparison.OrdinalIgnoreCase)
                    ? Labels.Help
                    : Labels.UnknownCommand;

                reply.Attachments.Add(new MainOptionsCard(!string.IsNullOrEmpty(data.TeamProject)));

                await context.PostAsync(reply);

                context.Wait(this.HandleActivityAsync);
            }
            else
            {
                this.telemetryClient.TrackEvent(result.Text);

                await context.Forward(dialog, this.ResumeAfterChildDialog, result, CancellationToken.None);
            }
        }

        /// <summary>
        /// Welcomes a user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task WelcomeAsync(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var message = result as IConversationUpdateActivity;
            if (message == null)
            {
                return;
            }

            if (!message.MembersAdded.Any() || message.MembersAdded.All(m => m.Id.Equals(message.Recipient.Id, StringComparison.OrdinalIgnoreCase)))
            {
                await context.PostAsync(string.Format(Labels.WelcomeUsers, this.licenseUri));
            }

            foreach (var member in message.MembersAdded)
            {
                // Skip if the added member is the bot itself.
                if (string.Equals(member.Id, message.Recipient.Id, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                await context.PostAsync(string.Format(Labels.WelcomeUser, member.Name, this.licenseUri));
            }
        }

        /// <summary>
        /// Resumes after a child dialog finishes.
        /// </summary>
        /// <param name="context">A result.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            try
            {
                await result;
            }
            catch (UnknownCommandException)
            {
                await this.HandleCommandAsync(context, context.Activity as IMessageActivity);
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                await context.PostAsync(string.Format(CultureInfo.CurrentCulture, Labels.ErrorOccurred, ex.Message));
            }
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.telemetryClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<TelemetryClient>();
        }
    }
}