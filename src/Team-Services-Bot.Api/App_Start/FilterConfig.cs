// ———————————————————————————————
// <copyright file="FilterConfig.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Registers global filters.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Registers global filters.
    /// </summary>
    public static class FilterConfig
    {
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            filters.Add(new HandleErrorAttribute());
        }
    }
}
