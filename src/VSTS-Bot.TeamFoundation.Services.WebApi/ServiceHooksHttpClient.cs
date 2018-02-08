// ———————————————————————————————
// <copyright file="ServiceHooksHttpClient.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Implementation of IServiceHooksHttpClient.
// </summary>
// ———————————————————————————————
namespace Vsar.TeamFoundation.Services.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    /// Implementation of <see cref="IServiceHooksHttpClient"/>.
    /// </summary>
    public class ServiceHooksHttpClient : VssHttpClientBase, IServiceHooksHttpClient
    {
        private readonly Guid locationId = new Guid("fc50d02a-849f-41fb-8af1-0a5216103269");
        private readonly ApiResourceVersion version = new ApiResourceVersion(1);

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHooksHttpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="credentials">The credentials.</param>
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials)
            : base(baseUrl, credentials)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHooksHttpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="settings">The request settings.</param>
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
            : base(baseUrl, credentials, settings)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHooksHttpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="handlers">The handlers.</param>
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, handlers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHooksHttpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="credentials">The credentials.</param>
        /// <param name="settings">The request settings.</param>
        /// <param name="handlers">The handlers.</param>
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, settings, handlers)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceHooksHttpClient"/> class.
        /// </summary>
        /// <param name="baseUrl">The base url.</param>
        /// <param name="pipeline">The pipeline.</param>
        /// <param name="disposeHandler">Dispose the handler.</param>
        public ServiceHooksHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
            : base(baseUrl, pipeline, disposeHandler)
        {
        }

        /// <inheritdoc />
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription)
        {
            return await this.CreateSubscriptionAsync(subscription, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription, object userState)
        {
            return await this.CreateSubscriptionAsync(subscription, userState, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription, object userState, CancellationToken cancellationToken)
        {
            return await this.PostAsync<Subscription, Subscription>(subscription, this.locationId, version: this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeleteSubscriptionAsync(Guid id)
        {
            await this.DeleteSubscriptionAsync(id, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task DeleteSubscriptionAsync(Guid id, object userState)
        {
            await this.DeleteSubscriptionAsync(id, userState, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task DeleteSubscriptionAsync(Guid id, object userState, CancellationToken cancellationToken)
        {
            var data = new { subscriptionId = id };

            await this.DeleteAsync(this.locationId, data, this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IList<Subscription>> GetSubscriptionsAsync()
        {
            return await this.GetSubscriptionsAsync(null, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<IList<Subscription>> GetSubscriptionsAsync(object userState)
        {
            return await this.GetSubscriptionsAsync(userState, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<IList<Subscription>> GetSubscriptionsAsync(object userState, CancellationToken cancellationToken)
        {
            return await this.SendAsync<IList<Subscription>>(new HttpMethod("GET"), this.locationId, version: this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Subscription> GetSubscriptionAsync(Guid id)
        {
            return await this.GetSubscriptionAsync(id, null, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<Subscription> GetSubscriptionAsync(Guid id, object userState)
        {
            return await this.GetSubscriptionAsync(id, userState, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<Subscription> GetSubscriptionAsync(Guid id, object userState, CancellationToken cancellationToken)
        {
            var data = new { subscriptionId = id };

            return await this.GetAsync<Subscription>(this.locationId, data, this.version, userState: userState, cancellationToken: cancellationToken);
        }
    }
}