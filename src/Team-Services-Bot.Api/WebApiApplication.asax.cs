// ———————————————————————————————
// <copyright file="WebApiApplication.asax.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Application startup process.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac.Integration.Mvc;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Represents the Start of the application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Method is called when the application starts.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed.")]
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["InstrumentationKey"];

            // Web API configuration and services
            var container = Bootstrap.Build();
            GlobalConfiguration.Configure(c => WebApiConfig.Register(c, container));
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}