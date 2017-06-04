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
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Autofac;
    using Autofac.Integration.Mvc;
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Represents the Start of the application.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Method is called when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["InstrumentationKey"];

            // Web API configuration and services
            var builder = new ContainerBuilder();
            builder
                .RegisterType<DialogInvoker>()
                .As<IDialogInvoker>();

            var container = Bootstrap.Build(builder, new WebConfigurationProvider(), this.Context.IsDebuggingEnabled);

            GlobalConfiguration.Configure(c => WebApiConfig.Register(c, container));
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}