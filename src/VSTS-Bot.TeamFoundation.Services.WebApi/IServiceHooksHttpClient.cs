// ———————————————————————————————
// <copyright file="IServiceHooksHttpClient.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Interface for handling VSTS Service Hooks subscriptions.
// </summary>
// ———————————————————————————————
namespace VSTS_Bot.TeamFoundation.Services.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for handling VSTS Service Hooks subscriptions.
    /// </summary>
    public interface IServiceHooksHttpClient
    {
        /// <summary>
        /// Creates a subscription in VSTS.
        /// </summary>
        /// <param name="subscription">The subscription to create.</param>
        /// <param name="userState">The userState.</param>
        /// <param name="cancellationToken">A token to cancel the async call.</param>
        /// <returns>The created subscription.</returns>
        Task<Subscription> CreateSubscriptionAsync(Subscription subscription, object userState = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Deletes the subscription in VSTS for the given subscription id.
        /// </summary>
        /// <param name="id">The id of the subscription.</param>
        /// <param name="userState">The userState.</param>
        /// <param name="cancellationToken">A token to cancel the async call.</param>
        /// <returns>A task.</returns>
        Task DeleteSubscriptionAsync(Guid id, object userState = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a list of subscriptions.
        /// </summary>
        /// <param name="userState">The userState.</param>
        /// <param name="cancellationToken">A token to cancel the async call.</param>
        /// <returns>The list of subscriptions.</returns>
        Task<IList<Subscription>> GetSubscriptionsAsync(object userState = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Gets a subscription from VSTS.
        /// </summary>
        /// <param name="id">The id of the subscription.</param>
        /// <param name="userState">The userState.</param>
        /// <param name="cancellationToken">A token to cancel the async call.</param>
        /// <returns>The subscription.</returns>
        Task<Subscription> GetSubscriptionAsync(Guid id, object userState = null, CancellationToken cancellationToken = default(CancellationToken));
    }
}