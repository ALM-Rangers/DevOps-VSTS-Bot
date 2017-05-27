// ———————————————————————————————
// <copyright file="VstsAccount.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a Team Services Account Information.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents information about VSTS Account
    /// </summary>
    [DataContract]
    public class VstsAccount
    {
        /// <summary>
        /// Gets or sets VSTS account name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets account URL. it should be in form https://account_name.visualstudio.com.
        /// </summary>
        [DataMember]
        public Uri Url { get; set; }

        /// <summary>
        /// Gets or sets the Token.
        /// </summary>
        [DataMember]
        public OAuthToken Token { get; set; }
    }
}