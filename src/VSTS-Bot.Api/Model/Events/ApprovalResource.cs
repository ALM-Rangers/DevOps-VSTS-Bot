// ———————————————————————————————
// <copyright file="ApprovalResource.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the resource of an event for approvals.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Events
{
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;

    /// <summary>
    /// Represents the resource of an event for approvals.
    /// </summary>
    public class ApprovalResource : Resource
    {
        /// <summary>
        /// Gets or sets the approval.
        /// </summary>
        public ReleaseApproval Approval { get; set; }
    }
}