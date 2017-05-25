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
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// Contains method(s) for accessing VSTS.
    /// </summary>
    public class VstsService : IVstsService
    {
        private const string VstsUrl = "https://{0}.visualstudio.com";
        private const string VstsRmUrl = "https://{0}.vsrm.visualstudio.com";

        private readonly Uri vstsAppUrl = new Uri("https://app.vssps.visualstudio.com");

        /// <inheritdoc />
        public async Task ChangeApprovalStatus(string account, string teamProject, VstsProfile profile, int approvalId, ApprovalStatus status, string comments)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            if (string.IsNullOrWhiteSpace(comments))
            {
                throw new ArgumentNullException(nameof(comments));
            }

            using (var client = this.GetConnectedClient<ReleaseHttpClient2>(new Uri(string.Format(VstsUrl, account)), profile.Token))
            {
                var approval = await client.GetApprovalAsync(teamProject, approvalId);
                approval.Status = status;
                approval.Comments = comments;

                await client.UpdateReleaseApprovalAsync(approval, teamProject, approvalId);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (var client = this.GetConnectedClient<AccountHttpClient>(this.vstsAppUrl, token))
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<ReleaseApproval>> GetApprovals(string account, string teamProject, VstsProfile profile)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            using (var client = this.GetConnectedClient<ReleaseHttpClient2>(new Uri(string.Format(VstsRmUrl, account)), profile.Token))
            {
                return await client.GetApprovalsAsync2(teamProject, profile.EmailAddress);
            }
        }

        /// <inheritdoc/>
        public async Task<Profile> GetProfile(OAuthToken token)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (var client = this.GetConnectedClient<ProfileHttpClient>(this.vstsAppUrl, token))
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

            using (var client = this.GetConnectedClient<ProjectHttpClient>(accountUrl, token))
            {
                return await client.GetProjects();
            }
        }

        private T GetConnectedClient<T>(Uri accountUri, OAuthToken token)
            where T : VssHttpClientBase
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(accountUri, credentials);
            var client = connection.GetClient<T>();

            return client;
        }
    }
}