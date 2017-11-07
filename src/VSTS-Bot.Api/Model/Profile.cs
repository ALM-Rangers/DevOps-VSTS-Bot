// ———————————————————————————————
// <copyright file="Profile.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a Visual Studio Team Services VSTSProfile.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a Visual Studio Team Services VSTSProfile.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "Reviewed.")]
    [DataContract]
    [Serializable]
    public class Profile
    {
        private string displayName;
        private OAuthToken token;

        /// <summary>
        /// Gets or sets a list of account names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed.")]
        [DataMember]
        public IList<string> Accounts { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName
        {
            get => this.displayName ?? (this.displayName = this.DisplayNameEncrypted.Decrypt<string>(this.Id.ToString()));
            set
            {
                this.displayName = value;
                this.DisplayNameEncrypted = value.Encrypt(this.Id.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the encrypted display name.
        /// </summary>
        [DataMember(Name = "DisplayName")]
        public string DisplayNameEncrypted { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is selected.
        /// </summary>
        [DataMember]
        public bool IsSelected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the profile is validated (by pin).
        /// </summary>
        [DataMember]
        public bool IsValidated { get; set; }

        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        public OAuthToken Token
        {
            get => this.token ?? (this.token = this.TokenEncrypted.Decrypt<OAuthToken>(this.Id.ToString()));
            set
            {
                this.token = value;
                this.TokenEncrypted = value.Encrypt(this.Id.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the encrypted token.
        /// </summary>
        [DataMember(Name = "Token")]
        public string TokenEncrypted { get; set; }
    }
}