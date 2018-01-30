// ———————————————————————————————
// <copyright file="DependencyResolverExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides extension methods for IDependencyResolver.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http.Dependencies;
    using Autofac;
    using Autofac.Features.Metadata;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Provides extension methods for <see cref="IDependencyResolver"/>.
    /// </summary>
    public static class DependencyResolverExtensions
    {
        /// <summary>
        /// Finds a <see cref="IDialog{TResult}" /> for the incoming activity text.
        /// </summary>
        /// <param name="resolver">A <see cref="IDependencyResolver"/>.</param>
        /// <param name="activityText">The text representing the command.</param>
        /// <returns>A <see cref="IDialog{TResult}"/>.</returns>
        public static IDialog<object> Find(this IDependencyScope resolver, string activityText)
        {
            resolver.ThrowIfNull(nameof(resolver));
            activityText.ThrowIfNullOrWhiteSpace(nameof(activityText));

            var dialogs = resolver.GetServices<Meta<IDialog<object>>>();

            return dialogs?
                .Where(m => ((string[])m.Metadata["Commands"]).Any(c => activityText.Trim().StartsWith(c, StringComparison.OrdinalIgnoreCase)))
                .Select(m => m.Value)
                .FirstOrDefault();
        }

        /// <summary>
        /// Retrieves a service from scope.
        /// </summary>
        /// <typeparam name="T">The returning type.</typeparam>
        /// <param name="resolver">The scope.</param>
        /// <returns>The service.</returns>
        public static T GetService<T>(this IDependencyScope resolver)
            where T : class
        {
            resolver.ThrowIfNull(nameof(resolver));

            return resolver.GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Retrieves services from scope.
        /// </summary>
        /// <typeparam name="T">The returning type.</typeparam>
        /// <param name="resolver">The scope.</param>
        /// <returns>The service.</returns>
        public static IEnumerable<T> GetServices<T>(this IDependencyScope resolver)
        {
            resolver.ThrowIfNull(nameof(resolver));

            return resolver.GetServices(typeof(T)) as IEnumerable<T>;
        }
    }
}