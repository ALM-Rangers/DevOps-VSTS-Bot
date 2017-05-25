// ———————————————————————————————
// <copyright file="IDialogContextWrapper.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for wrapping around the IDialogContext which makes it easier to do unit testing.
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
        /// Gets instance of <see cref="VstsService"/>
        /// </summary>
        IVstsService VstsService { get; }

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