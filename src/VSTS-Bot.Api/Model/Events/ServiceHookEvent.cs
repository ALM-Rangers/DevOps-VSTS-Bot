// ———————————————————————————————
// <copyright file="ServiceHookEvent.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an Service hook event.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Events
{
    /// <summary>
    /// Represents an Service hook event.
    /// </summary>
    /// <typeparam name="T">The event resource type.</typeparam>
    public class ServiceHookEvent<T> : ServiceHookEventBase
        where T : Resource
    {
        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        public T Resource { get; set; }
    }
}