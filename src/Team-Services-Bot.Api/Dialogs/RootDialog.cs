// ———————————————————————————————
// <copyright file="RootDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Root Dialog logic to handle messages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the Root dialog from where all conversations start.
    /// </summary>
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private bool initialized;

         /// <inheritdoc />
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var telemetryClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<TelemetryClient>();

            // Occurs when the conversation starts.
            if (!this.initialized)
            {
                this.initialized = true;
                await this.Welcome(context, activity);
            }
            else
            {
                await this.ProcessCommand(context, activity, telemetryClient);
            }
        }

        private async Task ProcessCommand(IDialogContext context, IMessageActivity activity, TelemetryClient telemetryClient)
        {
            var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find(activity.Text);

            if (dialog == null)
            {
                var reply = context.MakeMessage();

                reply.Attachments.Add(new MainOptionsCard());

                context.Wait(this.MessageReceivedAsync);
            }
            else
            {
                telemetryClient.TrackEvent(activity.Text);

                await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
            }
        }

        private async Task Welcome(IDialogContext context, IMessageActivity activity)
        {
            var account = context.UserData.GetCurrentAccount();
            var profile = context.UserData.GetProfile();
            var profiles = context.UserData.GetProfiles();
            var teamProject = context.UserData.GetCurrentTeamProject();

            if (string.IsNullOrWhiteSpace(account) || profile == null || !profiles.Any() ||
                string.IsNullOrWhiteSpace(teamProject))
            {
                await context.PostAsync(string.Format(Labels.WelcomeNewUser, activity.From.Name));

                var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find("connect");
                await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
            }
            else
            {
                await context.PostAsync(string.Format(Labels.WelcomeExistingUser, activity.From.Name, account, teamProject));
            }
        }

        private Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(this.MessageReceivedAsync);
            return Task.CompletedTask;
        }
    }
}