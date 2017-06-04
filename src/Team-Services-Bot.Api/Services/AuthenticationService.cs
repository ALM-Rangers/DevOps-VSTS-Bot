// ———————————————————————————————
// <copyright file="AuthenticationService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an implementation of IAuthenticationService.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// Represents an implementation of <see cref="IAuthenticationService"/>.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class AuthenticationService : IAuthenticationService
    {
        private const string FormatPostData =
            "client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&" +
            "client_assertion={0}&grant_type={1}&assertion={2}&redirect_uri={3}";

        private const string GrantTypeBearerToken = "urn:ietf:params:oauth:grant-type:jwt-bearer";
        private const string GrantTypeRefreshToken = "refresh_token";

        private const string MediaType = "application/x-www-form-urlencoded";
        private const string TokenUrl = "https://app.vssps.visualstudio.com/oauth2/token";

        private readonly string appSecret;
        private readonly Uri authorizeUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationService"/> class.
        /// </summary>
        /// <param name="appSecret">The appSecret.</param>
        /// <param name="authorizeUrl">The authorize url.</param>
        public AuthenticationService(string appSecret, Uri authorizeUrl)
        {
            this.appSecret = appSecret;
            this.authorizeUrl = authorizeUrl;
        }

        /// <inheritdoc />
        public async Task<OAuthToken> GetToken(string code)
        {
            var client = new HttpClient();
            var postData = string.Format(FormatPostData, HttpUtility.UrlEncode(this.appSecret), GrantTypeBearerToken, HttpUtility.UrlEncode(code), this.authorizeUrl);
            var response = await client
                .PostAsync(TokenUrl, new StringContent(postData, Encoding.UTF8, MediaType))
                .ConfigureAwait(false);

            return await response.Content.ReadAsAsync<OAuthToken>();
        }

        /// <inheritdoc />
        public async Task<OAuthToken> GetToken(OAuthToken token)
        {
            var client = new HttpClient();
            var postData = string.Format(FormatPostData, HttpUtility.UrlEncode(this.appSecret), GrantTypeRefreshToken, HttpUtility.UrlEncode(token.RefreshToken), this.authorizeUrl);
            var response = await client
                .PostAsync(TokenUrl, new StringContent(postData, Encoding.UTF8, MediaType))
                .ConfigureAwait(false);

            return await response.Content.ReadAsAsync<OAuthToken>();
        }
    }
}