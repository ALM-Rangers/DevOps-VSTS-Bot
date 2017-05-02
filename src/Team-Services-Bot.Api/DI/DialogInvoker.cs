// ———————————————————————————————
// <copyright file="DialogInvoker.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Concrete dialog invoker.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.DI
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Invokes dialogs.
    /// </summary>
    public class DialogInvoker : IDialogInvoker
    {
        /// <summary>
        /// Process an incoming message within the conversation.
        /// </summary>
        /// <param name="toBot">The message sent to the bot.</param>
        /// <param name="makeRoot">The factory method to make the root dialog.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the message to send inline back to the user.</returns>
        public async Task SendAsync(IMessageActivity toBot, Func<IDialog<object>> makeRoot, CancellationToken token)
        {
            await Conversation.SendAsync(toBot, makeRoot, token);
        }

        /// <summary>
        /// Process an incoming message within the conversation.
        /// </summary>
        /// <param name="toBot">The message sent to the bot.</param>
        /// <param name="makeRoot">The factory method to make the root dialog.</param>
        /// <returns>A task that represents the message to send inline back to the user.</returns>
        public async Task SendAsync(IMessageActivity toBot, Func<IDialog<object>> makeRoot)
        {
            await Conversation.SendAsync(toBot, makeRoot, default(CancellationToken));
        }
    }
}