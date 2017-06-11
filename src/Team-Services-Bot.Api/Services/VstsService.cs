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
    using System.Diagnostics.CodeAnalysis;
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
    [ExcludeFromCodeCoverage]
    public class VstsService : IVstsService
    {
        private const string VstsUrl = "https://{0}.visualstudio.com";
        private const string VstsRmUrl = "https://{0}.vsrm.visualstudio.com";

        private readonly Uri vstsAppUrl = new Uri("https://app.vssps.visualstudio.com");

        /// <inheritdoc />
        public async Task ChangeApprovalStatus(string account, string teamProject, VstsProfile profile, int approvalId, ApprovalStatus status, string comments)
        {
            if (account == null)
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

            using (var client = GetConnectedClient<ReleaseHttpClient2>(new Uri(string.Format(VstsUrl, account)), profile.Token))
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

            using (var client = GetConnectedClient<AccountHttpClient>(this.vstsAppUrl, token))
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <inheritdoc/>
        public async Task<ReleaseApproval> GetApproval(OAuthToken token, string account, string teamProject, int approvalId)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            if (approvalId <= 0)
            {
                throw new ArgumentNullException(nameof(approvalId));
            }

            using (var client = GetConnectedClient<ReleaseHttpClient2>(this.vstsAppUrl, token))
            {
                return await client.GetApprovalAsync(teamProject, approvalId);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<ReleaseApproval>> GetApprovals(string account, string teamProject, VstsProfile profile)
        {
            if (account == null)
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

            using (var client = GetConnectedClient<ReleaseHttpClient2>(new Uri(string.Format(VstsRmUrl, account)), profile.Token))
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

            using (var client = GetConnectedClient<ProfileHttpClient>(this.vstsAppUrl, token))
            {
                return await client.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TeamProjectReference>> GetProjects(string account, OAuthToken token)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (var client = GetConnectedClient<ProjectHttpClient>(new Uri(string.Format(VstsUrl, account)), token))
            {
                return await client.GetProjects();
            }
        }

        /// <inheritdoc />
        public async Task ReleaseQueue(string account, string teamProject, OAuthToken token, int definitionId)
        {
            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            using (var client = GetConnectedClient<ReleaseHttpClient2>(new Uri(string.Format(VstsRmUrl, account)), token))
            {
                var metaData = new ReleaseStartMetadata { DefinitionId = definitionId };
                await client.CreateReleaseAsync(metaData, teamProject);
            }
        }

        /// <summary>
        /// Gets a connected client.
        /// </summary>
        /// <typeparam name="T">The client type.</typeparam>
        /// <param name="accountUri">The url to the account.</param>
        /// <param name="token">The OAuth token.</param>
        /// <returns>A clientt.</returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot dispose the connection here. As it is used outside the scope.")]
        private static T GetConnectedClient<T>(Uri accountUri, OAuthToken token)
            where T : VssHttpClientBase
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));
            var connection = new VssConnection(accountUri, credentials);
            var client = connection.GetClient<T>();

            return client;
        }
    }
}