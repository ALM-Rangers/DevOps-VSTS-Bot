// ———————————————————————————————
// <copyright file="IEventStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for serviceHookEvent strategies.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Strategies.Events
{
    using System.Threading.Tasks;
    using TSBot.Events;
    using Subscription = TSBot.Subscription;

    /// <summary>
    /// Represents an interface for serviceHookEvent strategies.
    /// </summary>
    public interface IEventStrategy
    {
        /// <summary>
        /// Handles and serviceHookEvent.
        /// </summary>
        /// <param name="serviceHookEvent">the serviceHookEvent.</param>
        /// <param name="subscription">The matching subscription.</param>
        /// <returns>A task.</returns>
        Task Handle(ServiceHookEventBase serviceHookEvent, Subscription subscription);

        /// <summary>
        /// Checks if the serviceHookEvent should be handled by the strategy.
        /// </summary>
        /// <param name="serviceHookEvent">An serviceHookEvent.</param>
        /// <returns>A boolean value indicating whether the strategy should handle the serviceHookEvent.</returns>
        bool ShouldHandle(ServiceHookEventBase serviceHookEvent);
    }
}