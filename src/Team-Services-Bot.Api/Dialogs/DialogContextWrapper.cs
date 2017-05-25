// ———————————————————————————————
// <copyright file="DialogContextWrapper.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides an implementation for IDialogContextWrapper.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Provides an implementation for <see cref="IDialogContextWrapper"/>.
    /// </summary>
    public class DialogContextWrapper : IDialogContextWrapper
    {
        /// <inheritdoc />
        public IVstsService VstsService => new VstsService();

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