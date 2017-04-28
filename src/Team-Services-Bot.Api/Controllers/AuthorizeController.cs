// ———————————————————————————————
// <copyright file="AuthorizeController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Controller logic to handle the response from the oauth action.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.WebApi;
    using Resources;

    /// <summary>
    /// Handles the response from the oauth action.
    /// </summary>
    public class AuthorizeController : Controller
    {
        private const string FormatPostData =
                "client_assertion_type=urn:ietf:params:oauth:client-assertion-type:jwt-bearer&" +
                "client_assertion={0}&grant_type=urn:ietf:params:oauth:grant-type:jwt-bearer&assertion={1}&redirect_uri={2}";

        private const string MediaType = "application/x-www-form-urlencoded";
        private const string TokenUrl = "https://app.vssps.visualstudio.com/oauth2/token";

        private readonly string appSecret;
        private readonly Uri authorizeUrl;
        private readonly IBotState botState;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthorizeController"/> class.
        /// </summary>
        /// <param name="appSecret">The application secret.</param>
        /// <param name="authorizeUrl">The authorizationUrl.</param>
        /// <param name="telemetryClient">The TelemetryClient.</param>
        /// <param name="botState">The bot state.</param>
        public AuthorizeController(string appSecret, Uri authorizeUrl, IBotState botState, TelemetryClient telemetryClient)
        {
            this.appSecret = appSecret;
            this.authorizeUrl = authorizeUrl;
            this.botState = botState;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Handles the response for the oauth action.
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="error">A error.</param>
        /// <param name="state">The state.</param>
        /// <exception cref="ArgumentNullException">Occurs when code, error or state parameters are empty</exception>
        /// <returns>A view</returns>
        public async Task<ActionResult> Index(string code, string error, string state)
        {
            if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(error))
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrWhiteSpace(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            var stateArray = state.Split(';');

            if (stateArray.Length != 2)
            {
                throw new ArgumentException(Exceptions.InvalidState, nameof(state));
            }

            var channelId = stateArray[0];
            var userId = stateArray[1];

            try
            {
                // Get the security token.
                var client = new HttpClient();
                var postData = string.Format(FormatPostData, this.appSecret, code, this.authorizeUrl);
                var response = await client.PostAsync(TokenUrl, new StringContent(postData, Encoding.UTF8, MediaType));
                var token = await response.Content.ReadAsAsync<OAuthToken>();

                var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));

                var connection = new VssConnection(new Uri("https://app.vssps.visualstudio.com"), credentials);

                using (var pcl = connection.GetClient<ProfileHttpClient>())
                {
                    var profile = await pcl.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));

                    using (var acl = connection.GetClient<AccountHttpClient>())
                    {
                        var accounts = await acl.GetAccountsByMemberAsync(profile.Id);

                        var p = new VstsProfile
                        {
                            Accounts = accounts.Select(a => a.AccountName).ToList(),
                            Id = profile.Id
                        };

                        var data = await this.botState.GetUserDataAsync(channelId, userId);

                        data.SetProperty("VSTSProfile", p);

                        await this.botState.SetUserDataAsync(channelId, userId, data);
                    }
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                throw new Exception(Exceptions.UnknownException, ex);
            }

            return this.View();
        }
    }
}