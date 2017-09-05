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

    /// <summary>
    /// Represent VSTS Application registration information required to authenticate custom application by VSTS
    /// </summary>
    public class VstsApplication
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VstsApplication"/> class.
        /// </summary>
        /// <param name="id">Application ID</param>
        /// <param name="secret">Application secret</param>
        /// <param name="scope">Application scope</param>
        public VstsApplication(string id, string secret, string scope)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Secret = secret ?? throw new ArgumentNullException(nameof(secret));
            this.Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <summary>
        /// Gets application ID
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets application secret
        /// </summary>
        public string Secret { get; }

        /// <summary>
        /// Gets VSTS application scope
        /// </summary>
        public string Scope { get; }
    }
}