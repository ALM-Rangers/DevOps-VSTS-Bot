// ———————————————————————————————
// <copyright file="IDialogContextWrapper.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains interface for the wrapper
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Represents a wrapper.
    /// </summary>
    public interface IDialogContextWrapper
    {
        /// <summary>
        /// Gets the conversation data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="IBotDataBag"/>.</returns>
        IBotDataBag GetConversationData(IDialogContext context);

        /// <summary>
        /// Gets the private conversation data.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="IBotDataBag"/>.</returns>
        IBotDataBag GetPrivateConversationData(IDialogContext context);

        /// <summary>
        /// Gets the user data from the context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="IBotDataBag"/>.</returns>
        IBotDataBag GetUserData(IDialogContext context);
    }
}