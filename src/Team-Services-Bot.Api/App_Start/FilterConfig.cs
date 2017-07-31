// ———————————————————————————————
// <copyright file="FilterConfig.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides method(s) to register global filters.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;

    /// <summary>
    /// Provides method(s) to register global filters.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.ThrowIfNull(nameof(filters));

            filters.Add(new HandleErrorAttribute());
        }
    }
}
