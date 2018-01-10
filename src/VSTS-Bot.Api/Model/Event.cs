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

    /// <summary>
    /// Represents an Service hook event.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Gets or sets the id of the event.
        /// </summary>
        public Guid Id { get; set; }
    }
}