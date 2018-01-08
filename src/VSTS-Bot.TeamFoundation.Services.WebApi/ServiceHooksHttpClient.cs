// ———————————————————————————————
// <copyright file="ServiceHooksHttpClient.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Implementation of IServiceHooksHttpClient.
// </summary>
// ———————————————————————————————
namespace VSTS_Bot.TeamFoundation.Services.WebApi
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

        /// <inheritdoc />
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials)
            : base(baseUrl, credentials)
        {
        }

        /// <inheritdoc />
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
            : base(baseUrl, credentials, settings)
        {
        }

        /// <inheritdoc />
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, handlers)
        {
        }

        /// <inheritdoc />
        public ServiceHooksHttpClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings, params DelegatingHandler[] handlers)
            : base(baseUrl, credentials, settings, handlers)
        {
        }

        /// <inheritdoc />
        public ServiceHooksHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
            : base(baseUrl, pipeline, disposeHandler)
        {
        }

        /// <inheritdoc />
        public async Task<Subscription> CreateSubscriptionAsync(Subscription subscription, object userState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.PostAsync<Subscription, Subscription>(subscription, this.locationId, version: this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeleteSubscriptionAsync(Guid id, object userState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = new { subscriptionId = id };

            await this.DeleteAsync(this.locationId, data, this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IList<Subscription>> GetSubscriptionsAsync(object userState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await this.SendAsync<IList<Subscription>>(new HttpMethod("GET"), this.locationId, version: this.version, userState: userState, cancellationToken: cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Subscription> GetSubscriptionAsync(Guid id, object userState = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var data = new { subscriptionId = id };

            return await this.GetAsync<Subscription>(this.locationId, data, this.version, userState: userState, cancellationToken: cancellationToken);
        }
    }
}