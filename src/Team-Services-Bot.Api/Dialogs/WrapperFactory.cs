// ———————————————————————————————
// <copyright file="WrapperFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the wrapper factory.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Represents the wrapper factory.
    /// </summary>
    [Serializable]
    public class WrapperFactory : IWrapperFactory
    {
        /// <inheritdoc />
        public IWrapper Wrap(IDialogContext context)
        {
            return new Wrapper(context);
        }
    }
}