// ———————————————————————————————
// <copyright file="VstsApplication.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for retriving VSTS Application Registration based on session key.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Web.Http;

    /// <summary>
    /// Represent VSTS Application registration information required to authenticate custom application by VSTS
    /// </summary>
    public class VstsApplication : IVstsApplication
    {
        private readonly IAuthenticationServiceFactory authenticationServiceFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="VstsApplication"/> class.
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="secret">Application secret</param>
        /// <param name="scope">Application scope</param>
        /// <param name="redirectUri">Redirect URI</param>
        /// <param name="authenticationServiceFactory">The authentication service factory.</param>
        public VstsApplication(string id, string secret, string scope, Uri redirectUri, IAuthenticationServiceFactory authenticationServiceFactory)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Secret = secret ?? throw new ArgumentNullException(nameof(secret));
            this.Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this.RedirectUri = redirectUri ?? throw new ArgumentNullException(nameof(redirectUri));
            this.authenticationServiceFactory = authenticationServiceFactory ?? throw new ArgumentNullException(nameof(authenticationServiceFactory));
        }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public string Secret { get; }

        /// <inheritdoc />
        public string Scope { get; }

        /// <inheritdoc />
        public Uri RedirectUri { get; }

        /// <inheritdoc />
        public IAuthenticationService AuthenticationService => this.authenticationServiceFactory.GetService(this);
    }
}