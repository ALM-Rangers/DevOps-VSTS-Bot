// ———————————————————————————————
// <copyright file="EventController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Handles events from the VSTS Service hooks.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Events;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Resources;
    using Strategies.Events;

    /// <summary>
    /// Handles events from the VSTS Service hooks.
    /// </summary>
    public class EventController : ApiController
    {
        private readonly IDocumentClient documentClient;
        private readonly IEnumerable<IEventStrategy> strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="documentClient">The document client.</param>
        /// <param name="strategies">The event strategies.</param>
        public EventController(IDocumentClient documentClient, IEnumerable<IEventStrategy> strategies)
        {
            documentClient.ThrowIfNull(nameof(documentClient));
            strategies.ThrowIfNull(nameof(IEventStrategy));

            this.documentClient = documentClient;
            this.strategies = strategies;
        }

        /// <summary>
        /// Handles the events that are posted.
        /// </summary>
        /// <param name="event">The posted @event</param>
        /// <returns>Action results.</returns>
        public async Task<HttpResponseMessage> Post(ServiceHookEventBase @event)
        {
            try
            {
                if (!this.Request.Headers.Contains("subscriptionToken"))
                {
                    throw new Exception(Exceptions.MissingSubscriptionToken);
                }

                if (!Guid.TryParse(this.Request.Headers.GetValues("subscriptionToken").First(), out var subscriptionId))
                {
                    throw new Exception(Exceptions.InvalidSubscriptionToken);
                }

                var querySpec = new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM subscriptions s WHERE s.id = @id",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@id", subscriptionId),
                    }
                };

                var subscriptions = this.documentClient
                    .CreateDocumentQuery<Subscription>(
                        UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList();

                var tasks = new List<Task>();

                foreach (var subscription in subscriptions)
                {
                    foreach (var strategy in this.strategies.Where(s => s.ShouldHandle(@event)))
                    {
                        var t = strategy.Handle(@event, subscription);
                        tasks.Add(t);
                    }
                }

                await Task.WhenAll(tasks);

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }
    }
}