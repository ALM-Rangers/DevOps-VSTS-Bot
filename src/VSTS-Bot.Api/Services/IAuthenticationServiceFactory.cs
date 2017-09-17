// ———————————————————————————————
// <copyright file="IAuthenticationServiceFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an IAuthenticationService factory interface.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    /// <summary>
    /// IAuthenticationService factory interface.
    /// </summary>
    public interface IAuthenticationServiceFactory
    {
        /// <summary>
        /// Gets the instance of the authentication service.
        /// </summary>
        /// <param name="application">The <see cref="IVstsApplication"/> that represents VSTS application infromation.</param>
        /// <returns>The instance of authentication service.</returns>
        IAuthenticationService GetService(IVstsApplication application);
    }
}