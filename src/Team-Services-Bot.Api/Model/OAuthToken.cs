// ———————————————————————————————
// <copyright file="OAuthToken.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an OAuth token.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents the OAuth token.
    /// </summary>
    [DataContract]
    public class OAuthToken
    {
        /// <summary>
        /// Gets or sets the access tokens.
        /// </summary>
        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Gets or sets the time it expires in.
        /// </summary>
        [DataMember(Name = "expires_in")]
        public string ExpiresIn { get; set; }

        /// <summary>
        /// Gets or sets the refresh token.
        /// </summary>
        [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// Gets or sets the token type.
        /// </summary>
        [DataMember(Name = "token_type")]
        public string TokenType { get; set; }
    }
}