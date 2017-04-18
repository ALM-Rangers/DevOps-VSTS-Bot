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
    using Microsoft.ApplicationInsights.Extensibility;

    /// <summary>
    /// Represents the Start of the application.
    /// </summary>
    public class WebApiApplication : System.Web.HttpApplication
    {
        /// <summary>
        /// Method is called when the application starts.
        /// </summary>
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["InstrumentationKey"];

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
