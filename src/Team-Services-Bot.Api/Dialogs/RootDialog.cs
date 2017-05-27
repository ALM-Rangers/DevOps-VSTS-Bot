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
        [NonSerialized]
        private IDialogContextWrapper wrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialog"/> class.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        public RootDialog(IDialogContextWrapper wrapper)
        {
            this.wrapper = wrapper;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the dialog is initialized.
        /// </summary>
        public bool Initialized { get; set; }

        /// <inheritdoc />
        public Task StartAsync(IDialogContext context)
        {
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result, IBotDataBag userData)
        {
            var activity = await result;
            var telemetryClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<TelemetryClient>();

            // Occurs when the conversation starts.
            if (!this.Initialized)
            {
                this.Initialized = true;
                await this.Welcome(context, activity, userData);
            }
            else
            {
                await this.ProcessCommand(context, activity, telemetryClient);
            }
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.wrapper = GlobalConfiguration.Configuration.DependencyResolver.GetService<IDialogContextWrapper>();
        }

        private async Task ProcessCommand(IDialogContext context, IMessageActivity activity, TelemetryClient telemetryClient)
        {
            var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find(activity.Text);

            if (dialog == null)
            {
                var reply = context.MakeMessage();
                reply.Attachments.Add(new MainOptionsCard());

                await context.PostAsync(reply);

                context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));
            }
            else
            {
                telemetryClient.TrackEvent(activity.Text);

                await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
            }
        }

        private async Task Welcome(IDialogContext context, IMessageActivity activity, IBotDataBag userData)
        {
            var account = userData.GetCurrentAccount();
            var profile = userData.GetProfile();
            var profiles = userData.GetProfiles();
            var teamProject = userData.GetCurrentTeamProject();

            if (account == null || profile == null || !profiles.Any() || string.IsNullOrWhiteSpace(teamProject))
            {
                await context.PostAsync(string.Format(Labels.WelcomeNewUser, activity.From.Name));

                var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find("connect");

                if (dialog != null)
                {
                    await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
                }
            }
            else
            {
                await context.PostAsync(string.Format(Labels.WelcomeExistingUser, activity.From.Name, account, teamProject));
            }
        }

        private Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait((c, r) => this.MessageReceivedAsync(c, r, this.wrapper.GetUserData(c)));
            return Task.CompletedTask;
        }
    }
}