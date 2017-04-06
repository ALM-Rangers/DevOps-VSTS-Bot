//———————————————————————————————
// <copyright file=”name of this file, i.e. Global.asax.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Application startup process.
// </summary>
//———————————————————————————————
using System.Web.Configuration;
using System.Web.Http;
using Microsoft.ApplicationInsights.Extensibility;

namespace Vsar.TSBot
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TelemetryConfiguration.Active.InstrumentationKey = WebConfigurationManager.AppSettings["InstrumentationKey"];

            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
