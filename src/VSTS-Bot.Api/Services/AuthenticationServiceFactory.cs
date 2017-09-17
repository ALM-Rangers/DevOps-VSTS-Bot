// ———————————————————————————————
// <copyright file="AuthenticationServiceFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an IAuthenticationService factory interface.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Authentication Service factory
    /// </summary>
    public class AuthenticationServiceFactory : IAuthenticationServiceFactory
    {
        /// <inheritdoc />
        public IAuthenticationService GetService(IVstsApplication application)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new AuthenticationService(application.Secret, application.RedirectUri);
        }
    }
}