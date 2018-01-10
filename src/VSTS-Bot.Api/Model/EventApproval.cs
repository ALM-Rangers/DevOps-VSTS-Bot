// ———————————————————————————————
// <copyright file="EventApproval.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an approval in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents an approval in an event.
    /// </summary>
    public class EventApproval
    {
        /// <summary>
        /// Gets or sets the approval type.
        /// </summary>
        public string ApprovalType { get; set; }

        /// <summary>
        /// Gets or sets the approver.
        /// </summary>
        public EventApprover Approver { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the created on date.
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Gets or sets the id of the approval.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the notification is on.
        /// </summary>
        public bool IsNotificationOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the approval is automated.
        /// </summary>
        public bool IsAutomated { get; set; }

        /// <summary>
        /// Gets or sets the modified on date.
        /// </summary>
        public DateTime ModifiedOn { get; set; }

        /// <summary>
        /// Gets or sets the release.
        /// </summary>
        public EventRelease Release { get; set; }

        /// <summary>
        /// Gets or sets the release definition.
        /// </summary>
        public EventReleaseDefinition ReleaseDefinition { get; set; }

        /// <summary>
        /// Gets or sets the release environment.
        /// </summary>
        public EventReleaseEnvironment ReleaseEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        public int Revision { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the trial number.
        /// </summary>
        public int TrialNumber { get; set; }
    }
}