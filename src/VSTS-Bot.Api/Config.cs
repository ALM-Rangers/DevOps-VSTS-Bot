// ———————————————————————————————
// <copyright file="Config.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the configuration needed for the application.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.Web.Configuration;

    /// <summary>
    /// Represents the configuration needed for the application.
    /// </summary>
    public static class Config
    {
        /// <summary>
        /// Gets the Application Id for the VSTS registration of the application.
        /// </summary>
        public static string ApplicationId => WebConfigurationManager.AppSettings["AppId"];

        /// <summary>
        /// Gets the application scope for the VSTS registration.
        /// </summary>
        public static string ApplicationScope => WebConfigurationManager.AppSettings["AppScope"];

        /// <summary>
        /// Gets the Client Secret for the VSTS registration of the application.
        /// </summary>
        public static string ApplicationSecret => WebConfigurationManager.AppSettings["AppSecret"];

        /// <summary>
        /// Gets the Authorize Url.
        /// </summary>
        public static Uri AuthorizeUrl => new Uri(WebConfigurationManager.AppSettings["AuthorizeUrl"]);

        /// <summary>
        /// Gets the key for Document DB.
        /// </summary>
        public static string DocumentDbKey => WebConfigurationManager.AppSettings["DocumentDbKey"];

        /// <summary>
        /// Gets the url for Document DB
        /// </summary>
        public static Uri DocumentDbUrl => new Uri(WebConfigurationManager.AppSettings["DocumentDbUrl"]);

        /// <summary>
        /// Gets the instrumentation key for application insights.
        /// </summary>
        public static string InstrumentationKey => WebConfigurationManager.AppSettings["InstrumentationKey"];

        /// <summary>
        /// Gets the url to the emulator when debugging.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "EmulatorListeningUrl can be null or empty.")]
        public static string EmulatorListeningUrl => WebConfigurationManager.AppSettings["EmulatorListeningUrl"];

        /// <summary>
        /// Gets the Microsoft Application Id for the Bot Framework.
        /// </summary>
        public static string MicrosoftApplicationId => WebConfigurationManager.AppSettings["MicrosoftAppId"];

        /// <summary>
        /// Gets the Microsoft Application Password for the Bot Framework.
        /// </summary>
        public static string MicrosoftAppPassword => WebConfigurationManager.AppSettings["MicrosoftAppPassword"];
    }
}