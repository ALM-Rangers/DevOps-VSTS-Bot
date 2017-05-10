// ———————————————————————————————
// <copyright file="DialogContextWrapper.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the wrapper.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Represents the wrapper.
    /// </summary>
    public class DialogContextWrapper : IDialogContextWrapper
    {
        /// <inheritdoc />
        public IBotDataBag GetConversationData(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.ConversationData;
        }

        /// <inheritdoc />
        public IBotDataBag GetPrivateConversationData(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.PrivateConversationData;
        }

        /// <inheritdoc />
        public IBotDataBag GetUserData(IDialogContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.UserData;
        }
    }
}