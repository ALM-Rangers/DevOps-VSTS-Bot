// ———————————————————————————————
// <copyright file="IAuthenticationService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the interface for the AuthenticationService
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains the interface for the AuthenticationService.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gets the <see cref="OAuthToken"/>.
        /// </summary>
        /// <param name="code">The code we got from Vsts.</param>
        /// <returns><see cref="OAuthToken"/>.</returns>
        Task<OAuthToken> GetToken(string code);
    }
}