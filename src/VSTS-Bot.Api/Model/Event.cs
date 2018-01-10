// ———————————————————————————————
// <copyright file="Event.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an Service hook event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an Service hook event.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the detailed message.
        /// </summary>
        public EventMessage DetailedMessage { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the id of the event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public EventMessage Message { get; set; }

        /// <summary>
        /// Gets or sets the publisher id.
        /// </summary>
        public string PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the release.
        /// </summary>
        public EventRelease Release { get; set; }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public EventResource Resource { get; set; }

        /// <summary>
        /// Gets or sets the resource containers.
        /// </summary>
        public IDictionary<string, object> ResourceContainers { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Gets or sets the resource version.
        /// </summary>
        public string ResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public string Scope { get; set; }
    }
}