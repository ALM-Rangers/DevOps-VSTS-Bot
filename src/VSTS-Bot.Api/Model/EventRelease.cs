// ———————————————————————————————
// <copyright file="EventRelease.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a release in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents a release in an event.
    /// </summary>
    public class EventRelease
    {
        /// <summary>
        /// Gets or sets the id of the release.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the release.
        /// </summary>
        public string Name { get; set; }
    }
}