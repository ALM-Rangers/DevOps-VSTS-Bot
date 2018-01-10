// ———————————————————————————————
// <copyright file="EventApprover.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an approver in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents an approver in an event.
    /// </summary>
    public class EventApprover
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the id of the approver.
        /// </summary>
        public Guid Id { get; set; }
    }
}