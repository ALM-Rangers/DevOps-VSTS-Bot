//———————————————————————————————
// <copyright file=”name of this file, i.e. RootDialog.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Root Dialog logic to handle messages.
// </summary>
//———————————————————————————————

using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Vsar.TSBot.DI;

namespace Vsar.TSBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        [NonSerialized]
        private readonly IComponentContext _container;

        public RootDialog(IComponentContext container)
        {
            _container = container;
        }

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var dialog = _container.Find(activity.Text);

            if (dialog == null)
            {
                // TODO: Forward to the the help dialog.
                await context.PostAsync("Unknown command.");
            }
            else
            {
                await context.Forward(dialog, ResumeAfterChildDialog, activity, CancellationToken.None);
            }
        }

        private Task ResumeAfterChildDialog(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
    }
}