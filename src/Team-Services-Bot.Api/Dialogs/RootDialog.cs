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
        [NonSerialized]
        private IAuthenticationService authenticationService;
        [NonSerialized]
        private TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">A <see cref="IAuthenticationService"/>.</param>
        /// <param name="telemetryClient">A <see cref="telemetryClient"/>.</param>
        public RootDialog(IAuthenticationService authenticationService, TelemetryClient telemetryClient)
        {
            if (authenticationService == null)
            {
                throw new ArgumentNullException(nameof(authenticationService));
            }

            if (telemetryClient == null)
            {
                throw new ArgumentNullException(nameof(telemetryClient));
            }

            this.authenticationService = authenticationService;
            this.telemetryClient = telemetryClient;
        }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.HandleActivityAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Handles any incoming activity.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">a <see cref="IMessageActivity"/>.</param>
        /// <returns>a <see cref="Task"/>.</returns>
        public virtual async Task HandleActivityAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
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
        /// Handles any incoming command and route them to the appropiate dialog.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="activity">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task HandleCommandAsync(IDialogContext context, IMessageActivity activity)
        {
            var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find(activity.Text);

            if (dialog == null)
            {
                var reply = context.MakeMessage();
                reply.Attachments.Add(new MainOptionsCard());

                await context.PostAsync(reply);

                context.Wait(this.HandleActivityAsync);
            }
            else
            {
                this.telemetryClient.TrackEvent(activity.Text);

                await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
            }
        }

        /// <summary>
        /// Welcomes a user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="activity">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task WelcomeAsync(IDialogContext context, IMessageActivity activity)
        {
            var message = activity as IConversationUpdateActivity;
            if (message == null)
            {
                return;
            }

            var account = context.UserData.GetCurrentAccount();
            var profile = context.UserData.GetProfile(this.authenticationService);
            var teamProject = context.UserData.GetCurrentTeamProject();

            foreach (var member in message.MembersAdded)
            {
                // Skip if the added member is the bot itself.
                if (string.Equals(member.Id, message.Recipient.Id, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (profile == null)
                {
                    await context.PostAsync(string.Format(Labels.WelcomeNewUser, member.Name));
                }
                else
                {
                    await context.PostAsync(string.Format(Labels.WelcomeExistingUser, activity.From.Name, account ?? "?", teamProject ?? "?"));
                }
            }
        }

        /// <summary>
        /// Resumes after a child dialog finishes.
        /// </summary>
        /// <param name="context">A result.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.HandleActivityAsync);
            return Task.CompletedTask;
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.authenticationService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IAuthenticationService>();
            this.telemetryClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<TelemetryClient>();
        }
    }
}