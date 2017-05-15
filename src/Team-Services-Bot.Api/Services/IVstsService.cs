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
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;

    /// <summary>
    /// Interface for accessing VSTS.
    /// </summary>
    public interface IVstsService
    {
        /// <summary>
        /// Gets the accounts for which an user is a member.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.</param>
        /// <param name="memberId">The memberId.</param>
        /// <returns>A list with <see cref="Account"/>.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId);

        /// <summary>
        /// Gets the profile from vsts.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.s</param>
        /// <returns>A <see cref="Profile"/>.</returns>
        Task<Profile> GetProfile(OAuthToken token);

        /// <summary>
        /// Gets team projects from VSTS account
        /// </summary>
        /// <param name="accountUrl">The <see cref="Uri"/> that represents VSTS account URL.</param>
        /// <param name="token">The <see cref="OAuthToken"/>.</param>
        /// <returns>Collection of <see cref="TeamProjectReference"/></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IEnumerable<TeamProjectReference>> GetProjects(Uri accountUrl, OAuthToken token);
    }
}