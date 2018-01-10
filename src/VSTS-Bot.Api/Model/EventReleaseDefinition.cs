// ———————————————————————————————
// <copyright file="EventReleaseDefinition.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a release definition in an event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents a release definition in an event.
    /// </summary>
    public class EventReleaseDefinition
    {
        /// <summary>
        /// Gets or sets the id of the release definition.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the release definition.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the url of the release definition.
        /// </summary>
        public string Url { get; set; }
    }
}