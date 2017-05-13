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
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// Contains method(s) for accessing VSTS.
    /// </summary>
    public class VstsService : IVstsService
    {
        private const string Url = "https://app.vssps.visualstudio.com";

        /// <summary>
        /// Gets the VTST accounts for which an user is a member.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.</param>
        /// <param name="memberId">The memberId.</param>
        /// <returns>A list with <see cref="Account"/>.</returns>
        public async Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId)
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(new Uri(Url), credentials);

            using (var client = connection.GetClient<AccountHttpClient>())
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <summary>
        /// Gets the user profile from VSTS.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.s</param>
        /// <returns>A <see cref="Profile"/>.</returns>
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