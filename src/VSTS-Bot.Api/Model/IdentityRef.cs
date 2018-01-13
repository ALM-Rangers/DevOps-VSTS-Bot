// ———————————————————————————————
// <copyright file="IdentityRef.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an Identity in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents an Identity in an event.
    /// </summary>
    public class IdentityRef
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the id of the approver.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the url to the profile.
        /// </summary>
        public string ProfileUrl { get; set; }

        /// <summary>
        /// Gets or sets the url of the identity ref.
        /// </summary>
        public string Url { get; set; }
    }
}