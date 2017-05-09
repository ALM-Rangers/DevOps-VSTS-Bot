// ———————————————————————————————
// <copyright file="IWrapperFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains interface for the WrapperFactory
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Represents a factory for wrappers.
    /// </summary>
    public interface IWrapperFactory
    {
        /// <summary>
        /// Wraps a <see cref="IDialogContext"/>
        /// </summary>
        /// <param name="context">The <see cref="IDialogContext"/></param>
        /// <returns>A wrapper.</returns>
        IWrapper Wrap(IDialogContext context);
    }
}