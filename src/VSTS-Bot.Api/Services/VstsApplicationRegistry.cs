// ———————————————————————————————
// <copyright file="VstsApplicationRegistry.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents class for retriving VSTS Application Registration based on session key.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents class for retriving VSTS Application Registration based on session key
    /// </summary>
    public class VstsApplicationRegistry : IVstsApplicationRegistry
    {
        private readonly string applicationId;
        private readonly string applicationSecret;
        private readonly string applicationScope;
        private readonly Uri redirectUri;
        private readonly IAuthenticationServiceFactory authenticationServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VstsApplicationRegistry"/> class.
        /// </summary>
        /// <param name="applicationId">Application ID used by default.</param>
        /// <param name="applicationSecret">Application secret used by default.</param>
        /// <param name="applicationScope">Application scope used by defult.</param>
        /// <param name="redirectUri">Redirect URI.</param>
        /// <param name="authenticationServiceFactory">The authentication service factory.</param>
        public VstsApplicationRegistry(string applicationId, string applicationSecret, string applicationScope, Uri redirectUri, IAuthenticationServiceFactory authenticationServiceFactory)
        {
            this.applicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
            this.applicationSecret = applicationSecret ?? throw new ArgumentNullException(nameof(applicationSecret));
            this.applicationScope = applicationScope ?? throw new ArgumentNullException(nameof(applicationScope));
            this.redirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
            this.authenticationServiceFactory = authenticationServiceFactory ?? throw new ArgumentNullException(nameof(authenticationServiceFactory));
        }

        /// <inheritdoc />
        public IVstsApplication GetVstsApplicationRegistration(string userId)
        {
            if (userId == null)
            {
                throw new ArgumentNullException(nameof(userId));
            }

            return new VstsApplication(this.applicationId, this.applicationSecret, this.applicationScope, this.redirectUri, this.authenticationServiceFactory);
        }
    }
}