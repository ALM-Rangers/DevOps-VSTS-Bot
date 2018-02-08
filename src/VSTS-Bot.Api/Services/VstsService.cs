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
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.OAuth;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
    using Microsoft.VisualStudio.Services.WebApi;
    using TeamFoundation.Services.WebApi;

    /// <summary>
    /// Contains method(s) for accessing VSTS.
    /// </summary>
    public class VstsService : IVstsService
    {
        private const string VstsUrl = "https://{0}.visualstudio.com";
        private const string VstsRmUrl = "https://{0}.vsrm.visualstudio.com";

        private readonly Uri vstsAppUrl = new Uri("https://app.vssps.visualstudio.com");

        /// <inheritdoc />
        public async Task ChangeApprovalStatus(string account, string teamProject, Profile profile, int approvalId, ApprovalStatus status, string comments)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            profile.ThrowIfNull(nameof(profile));

            var token = profile.Token;

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                var approval = await client.GetApprovalAsync(teamProject, approvalId);
                approval.Status = status;
                approval.Comments = comments;

                await client.UpdateReleaseApprovalAsync(approval, teamProject, approvalId);
            }
        }

        /// <inheritdoc />
        public async Task<Release> CreateReleaseAsync(string account, string teamProject, int definitionId, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            definitionId.ThrowIfSmallerOrEqual(nameof(definitionId));
            token.ThrowIfNull(nameof(token));

            ReleaseDefinition definition;

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                definition = await client.GetReleaseDefinitionAsync(teamProject, definitionId);
            }

            var metadatas = new List<ArtifactMetadata>();

            using (var client = await this.ConnectAsync<BuildHttpClient>(token, account))
            {
                foreach (var artifact in definition.Artifacts.Where(a => string.Equals(a.Type, ArtifactTypes.BuildArtifactType, StringComparison.OrdinalIgnoreCase)))
                {
                    var definitions = new List<int> { Convert.ToInt32(artifact.DefinitionReference["definition"].Id) };
                    var builds = await client.GetBuildsAsync2(teamProject, definitions);
                    var build = builds.OrderByDescending(b => b.LastChangedDate).FirstOrDefault();

                    if (build == null)
                    {
                        continue;
                    }

                    var metadata = new ArtifactMetadata
                    {
                        Alias = artifact.Alias,
                        InstanceReference = new BuildVersion { Id = build.Id.ToString() }
                    };

                    metadatas.Add(metadata);
                }
            }

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                var metaData = new ReleaseStartMetadata { DefinitionId = definitionId, Artifacts = metadatas };
                return await client.CreateReleaseAsync(metaData, teamProject);
            }
        }

        /// <inheritdoc />
        public async Task<TeamFoundation.Services.WebApi.Subscription> CreateSubscription(string account, TeamFoundation.Services.WebApi.Subscription subscription, OAuthToken token)
        {
            account.ThrowIfNull(nameof(account));
            subscription.ThrowIfNull(nameof(subscription));
            token.ThrowIfNull(nameof(token));

            var isRm = subscription.PublisherId.Equals("RM", StringComparison.OrdinalIgnoreCase);

            using (var client = await this.ConnectAsync<ServiceHooksHttpClient>(token, account, isRm))
            {
                return await client.CreateSubscriptionAsync(subscription);
            }
        }

        /// <inheritdoc />
        public async Task DeleteSubscription(string account, Guid subscriptionId, OAuthToken token)
        {
            account.ThrowIfNull(nameof(account));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ServiceHooksHttpClient>(token, account))
            {
                await client.DeleteSubscriptionAsync(subscriptionId);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId)
        {
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<AccountHttpClient>(token))
            {
                return await client.GetAccountsByMemberAsync(memberId);
            }
        }

        /// <inheritdoc/>
        public async Task<ReleaseApproval> GetApproval(string account, string teamProject, int approvalId, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            approvalId.ThrowIfSmallerOrEqual(nameof(approvalId));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                return await client.GetApprovalAsync(teamProject, approvalId);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<ReleaseApproval>> GetApprovals(string account, string teamProject, Profile profile)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            profile.ThrowIfNull(nameof(profile));

            var p = await this.GetProfile(profile.Token);

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(profile.Token, account))
            {
                return await client.GetApprovalsAsync2(teamProject, p.DisplayName, includeMyGroupApprovals: true);
            }
        }

        /// <inheritdoc/>
        public async Task<Build> GetBuildAsync(string account, string teamProject, int id, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            id.ThrowIfSmallerOrEqual(nameof(id));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<BuildHttpClient>(token, account))
            {
                return await client.GetBuildAsync(teamProject, id);
            }
        }

        /// <inheritdoc/>
        public async Task<IList<BuildDefinitionReference>> GetBuildDefinitionsAsync(string account, string teamProject, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<BuildHttpClient>(token, account))
            {
                return await client.GetDefinitionsAsync(teamProject, name: null);
            }
        }

        /// <inheritdoc/>
        public async Task<Microsoft.VisualStudio.Services.Profile.Profile> GetProfile(OAuthToken token)
        {
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ProfileHttpClient>(token))
            {
                return await client.GetProfileAsync(new ProfileQueryContext(AttributesScope.Core));
            }
        }

        /// <inheritdoc />
        public async Task<IList<TeamProjectReference>> GetProjects(string account, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ProjectHttpClient>(token, account))
            {
                var results = await client.GetProjects();
                return results.ToList();
            }
        }

        /// <inheritdoc />
        public async Task<IList<ReleaseDefinition>> GetReleaseDefinitionsAsync(string account, string teamProject, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                return await client.GetReleaseDefinitionsAsync(teamProject);
            }
        }

        /// <inheritdoc />
        public async Task<TeamFoundation.Services.WebApi.Subscription> GetSubscription(string account, Guid subscriptionId, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ServiceHooksHttpClient>(token, account))
            {
                return await client.GetSubscriptionAsync(subscriptionId);
            }
        }

        /// <inheritdoc />
        public async Task<IList<TeamFoundation.Services.WebApi.Subscription>> GetSubscriptions(string account, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ServiceHooksHttpClient>(token, account))
            {
                return await client.GetSubscriptionsAsync();
            }
        }

        /// <inheritdoc />
        public async Task<Build> QueueBuildAsync(string account, string teamProject, int definitionId, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            definitionId.ThrowIfSmallerOrEqual(nameof(definitionId));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<BuildHttpClient>(token, account))
            {
                var build = new Build { Definition = new BuildDefinitionReference { Id = definitionId } };

                return await client.QueueBuildAsync(build, teamProject);
            }
        }

        /// <inheritdoc />
        public async Task<Release> GetReleaseAsync(string account, string teamProject, int id, OAuthToken token)
        {
            account.ThrowIfNullOrWhiteSpace(nameof(account));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));
            id.ThrowIfSmallerOrEqual(nameof(id));
            token.ThrowIfNull(nameof(token));

            using (var client = await this.ConnectAsync<ReleaseHttpClient2>(token, account))
            {
                return await client.GetReleaseAsync(teamProject, id);
            }
        }

        /// <summary>
        /// Connects to a client.
        /// </summary>
        /// <typeparam name="T">The client type.</typeparam>
        /// <param name="token">The OAuth token.</param>
        /// <param name="account">The name of the account to connect to.</param>
        /// <param name="isRm">Forces to connect to the rm url.</param>
        /// <returns>A client.</returns>
        private async Task<T> ConnectAsync<T>(OAuthToken token, string account = null, bool isRm = false)
            where T : VssHttpClientBase
        {
            var credentials = new VssOAuthAccessTokenCredential(new VssOAuthAccessToken(token.AccessToken));

            var uri = this.vstsAppUrl;

            if (!string.IsNullOrWhiteSpace(account))
            {
                uri = isRm
                    ? new Uri(string.Format(CultureInfo.InvariantCulture, VstsRmUrl, HttpUtility.UrlEncode(account)))
                    : new Uri(string.Format(CultureInfo.InvariantCulture, VstsUrl, HttpUtility.UrlEncode(account)));
            }

            return await new VssConnection(uri, credentials).GetClientAsync<T>();
        }
    }
}