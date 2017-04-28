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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the Root dialog from where all conversations start.
    /// </summary>
    [Serializable]
    public class RootDialog : IDialog<object>
    {
         /// <inheritdoc />
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var dialog = GlobalConfiguration.Configuration.DependencyResolver.Find(activity.Text);
            var telemetryClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<TelemetryClient>();

            if (dialog == null)
            {
                // TODO: Forward to the help dialog.
                await context.PostAsync("Unknown command.");
            }
            else
            {
                telemetryClient.TrackEvent(activity.Text);

                await context.Forward(dialog, this.ResumeAfterChildDialog, activity, CancellationToken.None);
            }
        }

        private Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            return Task.CompletedTask;
        }
    }
}