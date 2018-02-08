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

        /// <inheritdoc />
        public async Task<OAuthToken> GetToken(string appSecret, Uri authorizeUrl, string code)
        {
            var postData = string.Format(FormatPostData, HttpUtility.UrlEncode(appSecret), GrantTypeBearerToken, HttpUtility.UrlEncode(code), authorizeUrl);

            using (var client = new HttpClient())
            {
                var response = await client
                    .PostAsync(TokenUrl, new StringContent(postData, Encoding.UTF8, MediaType))
                    .ConfigureAwait(false);

                var result = await response.Content.ReadAsAsync<OAuthToken>();
                result.AppSecret = appSecret;
                result.AuthorizeUrl = authorizeUrl;
                return result;
            }
        }

        /// <inheritdoc />
        public async Task<OAuthToken> GetToken(OAuthToken token)
        {
            var postData = string.Format(FormatPostData, HttpUtility.UrlEncode(token.AppSecret), GrantTypeRefreshToken, HttpUtility.UrlEncode(token.RefreshToken), token.AuthorizeUrl);

            using (var client = new HttpClient())
            {
                var response = await client
                    .PostAsync(TokenUrl, new StringContent(postData, Encoding.UTF8, MediaType))
                    .ConfigureAwait(false);

                var result = await response.Content.ReadAsAsync<OAuthToken>();
                result.AppSecret = token.AppSecret;
                result.AuthorizeUrl = token.AuthorizeUrl;
                return result;
            }
        }
    }
}