// ———————————————————————————————
// <copyright file="ProfileService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an implementation of IProfileService.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// Represents an implementation of <see cref="IProfileService"/>.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private const string Url = "https://app.vssps.visualstudio.com";

        /// <inheritdoc />
        public async Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId)
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(new Uri(Url), credentials);

            using (var client = connection.GetClient<AccountHttpClient>())
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <inheritdoc />
        public async Task<Profile> GetProfile(OAuthToken token)
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(new Uri(Url), credentials);

            using (var client = connection.GetClient<ProfileHttpClient>())
            {
                return await client.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
            }
        }
    }
}