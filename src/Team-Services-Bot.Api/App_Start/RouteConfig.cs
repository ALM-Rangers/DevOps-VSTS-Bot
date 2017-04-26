// ———————————————————————————————
// <copyright file="RouteConfig.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the routing configuration.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Mvc;
    using System.Web.Routing;

    /// <summary>
    /// Contains the routing configuration.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The <see cref="RouteCollection"/>.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("Default", "{controller}/{action}/{id}", new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
