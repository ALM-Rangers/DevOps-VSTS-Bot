// ———————————————————————————————
// <copyright file="VSTSProfile.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a Team Services VSTSProfile.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a Team Services VSTSProfile.
    /// </summary>
    [DataContract]
    public class VstsProfile
    {
        /// <summary>
        /// Gets or sets the access tokens.
        /// </summary>
        [DataMember]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets a list of account names.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Reviewed.")]
        [DataMember]
        public IList<string> Accounts { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the time it expires in.
        /// </summary>
        [DataMember]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [DataMember]
        public string TokenType { get; set; }
    }
}