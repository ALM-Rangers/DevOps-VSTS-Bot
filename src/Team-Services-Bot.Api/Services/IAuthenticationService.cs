// ———————————————————————————————
// <copyright file="IAuthenticationService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for handling authentication with VSTS.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an interface for handling authentication with VSTS.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Gets the <see cref="OAuthToken"/>.
        /// </summary>
        /// <param name="code">The code we got from Vsts.</param>
        /// <returns><see cref="OAuthToken"/>.</returns>
        Task<OAuthToken> GetToken(string code);

        /// <summary>
        /// Gets the <see cref="OAuthToken"/>.
        /// </summary>
        /// <param name="token">Based on the refreshToken.</param>
        /// <returns>An updated <see cref="OAuthToken"/>.</returns>
        Task<OAuthToken> GetToken(OAuthToken token);
    }
}