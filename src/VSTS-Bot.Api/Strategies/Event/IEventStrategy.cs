// ———————————————————————————————
// <copyright file="IEventStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for event strategies.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Strategies.Event
{
    using System.Threading.Tasks;
    using Events;
    using Subscription = TSBot.Subscription;

    /// <summary>
    /// Represents an interface for event strategies.
    /// </summary>
    public interface IEventStrategy
    {
        /// <summary>
        /// Handles and event.
        /// </summary>
        /// <param name="event">the event.</param>
        /// <param name="subscription">The matching subscription.</param>
        /// <returns>A task.</returns>
        Task Handle(EventBase @event, Subscription subscription);

        /// <summary>
        /// Checks if the event should be handled by the strategy.
        /// </summary>
        /// <param name="event">An event.</param>
        /// <returns>A boolean value indicating whether the strategy should handle the event.</returns>
        bool ShouldHandle(EventBase @event);
    }
}