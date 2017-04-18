// ———————————————————————————————
// <copyright file="ComponentContextExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains extension methods for IComponentContext.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac;
    using Autofac.Features.Metadata;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Contains extensions for <see cref="IComponentContext"/>/
    /// </summary>
    public static class ComponentContextExtensions
    {
        /// <summary>
        /// Finds a <see cref="IDialog{TResult}" /> for the incoming activity text.
        /// </summary>
        /// <param name="context">A <see cref="IComponentContext"/>.</param>
        /// <param name="activityText">The text representing the command.</param>
        /// <returns>A <see cref="IDialog{TResult}"/>.</returns>
        public static IDialog<object> Find(this IComponentContext context, string activityText)
        {
            var dialogs = context.Resolve<IEnumerable<Meta<IDialog<object>>>>();

            return dialogs
                .Where(m => activityText.StartsWith(m.Metadata["Command"].ToString(), StringComparison.OrdinalIgnoreCase))
                .Select(m => m.Value)
                .FirstOrDefault();
        }
    }
}