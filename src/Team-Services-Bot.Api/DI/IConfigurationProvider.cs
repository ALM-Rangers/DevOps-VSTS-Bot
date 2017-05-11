// ———————————————————————————————
// <copyright file="IConfigurationProvider.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Interface used to access configuration.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.DI
{
    /// <summary>
    /// Defines the operations for accessing configuration values.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets the value of the named configuration item.
        /// </summary>
        /// <param name="name">The name of the configuration item to get.</param>
        /// <returns>The configuration value, <c>null</c> if the named item does not exist.</returns>
        string GetValue(string name);
    }
}
