// ———————————————————————————————
// <copyright file="IDialogInvoker.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Interface used to invoke dialogs.
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
    /// Defines operations used to invoked dialogs
    /// </summary>
    public interface IDialogInvoker
    {
        /// <summary>
        /// Process an incoming message within the conversation.
        /// </summary>
        /// <param name="toBot">The message sent to the bot.</param>
        /// <param name="makeRoot">The factory method to make the root dialog.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A task that represents the message to send inline back to the user.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by Bot framework")]
        Task SendAsync(IMessageActivity toBot, Func<IDialog<object>> makeRoot, CancellationToken token);

        /// <summary>
        /// Process an incoming message within the conversation.
        /// </summary>
        /// <param name="toBot">The message sent to the bot.</param>
        /// <param name="makeRoot">The factory method to make the root dialog.</param>
        /// <returns>A task that represents the message to send inline back to the user.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Required by Bot framework")]
        Task SendAsync(IMessageActivity toBot, Func<IDialog<object>> makeRoot);
    }
}
