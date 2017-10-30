    // ———————————————————————————————
// <copyright file="VstsProfile.cs">
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
    [DataContract]
    [Serializable]
    public class VstsProfile
    {
        /// <summary>
        /// Gets or sets a list of account names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed.")]
        [DataMember]
        public IList<string> Accounts { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [DataMember]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        [DataMember]
        public OAuthToken Token { get; set; }
    }
}