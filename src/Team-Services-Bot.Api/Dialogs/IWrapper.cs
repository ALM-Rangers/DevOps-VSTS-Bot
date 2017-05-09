// ———————————————————————————————
// <copyright file="IWrapper.cs">
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
    public interface IWrapper
    {
        /// <summary>
        /// Gets the context.
        /// </summary>
        IDialogContext Context { get; }

        /// <summary>
        /// Gets the userdata.
        /// </summary>
        IBotDataBag UserData { get; }
    }
}