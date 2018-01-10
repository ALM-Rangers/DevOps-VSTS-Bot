// ———————————————————————————————
// <copyright file="EventReleaseEnvironment.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a release environment in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents a release environment in an event.
    /// </summary>
    public class EventReleaseEnvironment
    {
        /// <summary>
        /// Gets or sets the id of the Release Evironment.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the release environment.
        /// </summary>
        public string Name { get; set; }
    }
}