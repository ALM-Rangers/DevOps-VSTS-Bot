//———————————————————————————————
// <copyright file=”name of this file, i.e. EchoDialog.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// <summary>
// Echo's the messages from the user.
// </summary>
//———————————————————————————————

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Vsar.TSBot.DI;

namespace Vsar.TSBot.Dialogs
{
    [CommandMetadata("Test"), Serializable]
    public class EchoDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            var length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
}