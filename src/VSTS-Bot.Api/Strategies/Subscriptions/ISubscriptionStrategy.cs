// ———————————————————————————————
// <copyright file="ISubscriptionStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Interface for subscription strategies.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Strategies.Subscriptions
{
    using System;
    using Microsoft.TeamFoundation.Core.WebApi;
    using TeamFoundation.Services.WebApi;

    /// <summary>
    /// Interface for subscription strategies.
    /// </summary>
    public interface ISubscriptionStrategy
    {
        /// <summary>
        /// Checks if this strategy can get a subscription for the given subscription type.
        /// </summary>
        /// <param name="subscriptionType">The subscription type.</param>
        /// <returns>A boolean value indicating whether the strategy can get a subscription.</returns>
        bool CanGetSubscription(SubscriptionType subscriptionType);

        /// <summary>
        /// Gets the subscription.
        /// </summary>
        /// <param name="subscriptionId">The subscription id.</param>
        /// <param name="teamProject">The team project.</param>
        /// <returns>A Subscription.</returns>
        Subscription GetSubscription(Guid subscriptionId, TeamProjectReference teamProject);
    }
}