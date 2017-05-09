// ———————————————————————————————
// <copyright file="Wrapper.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the wrapper.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Represents the wrapper.
    /// </summary>
    public class Wrapper : IWrapper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Wrapper"/> class.
        /// </summary>
        /// <param name="context">The <see cref="IDialogContext"/></param>
        public Wrapper(IDialogContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets the cotext.
        /// </summary>
        public IDialogContext Context { get; }

        /// <summary>
        /// Gets the UserData.
        /// </summary>
        public IBotDataBag UserData => this.Context.UserData;
    }
}