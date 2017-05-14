// ———————————————————————————————
// <copyright file="VstsService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the ProfileService
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// Contains method(s) for accessing VSTS.
    /// </summary>
    public class VstsService : IVstsService
    {
        private readonly Uri vstsAppUrl = new Uri("https://app.vssps.visualstudio.com");

        /// <inheritdoc/>
        public async Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (AccountHttpClient client = ConnectAndGetClient<AccountHttpClient>(this.vstsAppUrl, token))
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <inheritdoc/>
        public async Task<Profile> GetProfile(OAuthToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (ProfileHttpClient client = ConnectAndGetClient<ProfileHttpClient>(this.vstsAppUrl, token))
            {
                return await client.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamProjectReference>> GetProjects(Uri accountUrl, OAuthToken token)
        {
            if (accountUrl == null)
            {
                throw new ArgumentNullException(nameof(accountUrl));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (ProjectHttpClient client = ConnectAndGetClient<ProjectHttpClient>(accountUrl, token))
            {
                return await client.GetProjects();
            }
        }

        private static T ConnectAndGetClient<T>(Uri accountUri, OAuthToken token)
            where T : VssHttpClientBase
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(accountUri, credentials);
            T client = connection.GetClient<T>();

            return client;
        }
    }
}