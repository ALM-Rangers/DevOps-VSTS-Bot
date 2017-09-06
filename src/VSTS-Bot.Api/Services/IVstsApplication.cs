// ———————————————————————————————
// <copyright file="IVstsApplication.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for retriving VSTS Application Registration based on session key.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represent VSTS Application registration information required to authenticate custom application by VSTS
    /// </summary>
    public interface IVstsApplication
    {
        /// <summary>
        /// Gets application ID
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets application secret
        /// </summary>
        string Secret { get; }

        /// <summary>
        /// Gets VSTS application scope
        /// </summary>
        string Scope { get; }

        /// <summary>
        /// Gets redirect URI
        /// </summary>
        Uri RedirectUri { get; }

        /// <summary>
        /// Gets instance of <see cref="IAuthenticationService"/> to be used to authenticate against application.
        /// </summary>
        IAuthenticationService AuthenticationService { get; }
    }
}