// ———————————————————————————————
// <copyright file="IVstsService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for getting profile information for a user from VSTS.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;

    /// <summary>
    /// Interface for accessing VSTS.
    /// </summary>
    public interface IVstsService
    {
        /// <summary>
        /// Changes the status for an approval.
        /// </summary>
        /// <param name="account">The VSTS account.</param>
        /// <param name="teamProject">The team project.</param>
        /// <param name="profile">The user profile.</param>
        /// <param name="approvalId">The id of the approval.</param>
        /// <param name="status">The approval status.</param>
        /// <param name="comments">A comment.</param>
        /// <returns>A void task.</returns>
        Task ChangeApprovalStatus(string account, string teamProject, VstsProfile profile, int approvalId, ApprovalStatus status, string comments);

        /// <summary>
        /// Gets the accounts for which an user is a member.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.</param>
        /// <param name="memberId">The memberId.</param>
        /// <returns>A list with <see cref="Account"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId);

        /// <summary>
        /// Gets an <see cref="ReleaseApproval"/> by its id.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/></param>
        /// <param name="account">The VSTS account.</param>
        /// <param name="teamProject">The team project.</param>
        /// <param name="approvalId">The approval id.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task<ReleaseApproval> GetApproval(OAuthToken token, string account, string teamProject, int approvalId);

        /// <summary>
        /// Gets the approvals for the user.
        /// </summary>
        /// <param name="account">An account.</param>
        /// <param name="teamProject">The team project.</param>
        /// <param name="profile">The profile of the user.</param>
        /// <returns>A list with <see cref="ReleaseApproval"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "We are implementing async function, so nesting is ok.")]
        Task<IList<ReleaseApproval>> GetApprovals(string account, string teamProject, VstsProfile profile);

        /// <summary>
        /// Gets the profile from vsts.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.s</param>
        /// <returns>A <see cref="Profile"/>.</returns>
        Task<Profile> GetProfile(OAuthToken token);

        /// <summary>
        /// Gets team projects from VSTS account
        /// </summary>
        /// <param name="account">The VSTS account name</param>
        /// <param name="token">The <see cref="OAuthToken"/> for authentication.</param>
        /// <returns>Collection of <see cref="TeamProjectReference"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IEnumerable<TeamProjectReference>> GetProjects(string account, OAuthToken token);

        /// <summary>
        /// Gets build definitions from VSTS account for specified project
        /// </summary>
        /// <param name="project">The Team Project name</param>
        /// <param name="account">The VSTS account name</param>
        /// <param name="token">The <see cref="OAuthToken"/> for authentication.</param>
        /// <returns>Collection of <see cref="BuildDefinitionReference"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IEnumerable<BuildDefinitionReference>> GetBuildDefinitionsAsync(string project, string account, OAuthToken token);

        /// <summary>
        /// Queues a release.
        /// </summary>
        /// <param name="account">The <see cref="Uri"/> that represents VSTS account URL.</param>
        /// <param name="teamProject">The team project.</param>
        /// <param name="token">The <see cref="OAuthToken"/>.</param>
        /// <param name="definitionId">The name of a release definition.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        Task ReleaseQueueAsync(string account, string teamProject, OAuthToken token, int definitionId);
    }
}