// ———————————————————————————————
// <copyright file="IVstsService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the interface for the ProfileService
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;

    /// <summary>
    /// Interface for the ProfileService implementation.
    /// </summary>
    public interface IVstsService
    {
        /// <summary>
        /// Gets the accounts for which an user is a member.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.</param>
        /// <param name="memberId">The memberId.</param>
        /// <returns>A list with accounts.s</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed.")]
        Task<IList<Account>> GetAccounts(OAuthToken token, Guid memberId);

        /// <summary>
        /// Gets the profile from vsts.
        /// </summary>
        /// <param name="token">A <see cref="OAuthToken"/>.s</param>
        /// <returns>A <see cref="Microsoft.VisualStudio.Services.Profile.Profile"/>.</returns>
        Task<Profile> GetProfile(OAuthToken token);
    }
}