// ———————————————————————————————
// <copyright file="WebConfigurationProvider.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provider to access configuration for a web application.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.DI
{
    using System.Web.Configuration;

    /// <summary>
    /// Provides configuration to a web application.
    /// </summary>
    public class WebConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Gets the value of the named configuration item.
        /// </summary>
        /// <param name="name">The name of the configuration item to get.</param>
        /// <returns>The configuration value, <c>null</c> if the named item does not exist.</returns>
        public string GetValue(string name)
        {
            return WebConfigurationManager.AppSettings[name];
        }
    }
}