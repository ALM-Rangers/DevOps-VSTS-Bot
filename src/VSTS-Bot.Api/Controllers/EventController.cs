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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Handles events from the VSTS Service hooks.
    /// </summary>
    public class EventController : ApiController
    {
        private readonly MicrosoftAppCredentials credentials;
        private readonly IDocumentClient documentClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="credentials">The Microsoft Application Credentials.</param>
        /// <param name="documentClient">The document client.</param>
        public EventController(MicrosoftAppCredentials credentials, IDocumentClient documentClient)
        {
            credentials.ThrowIfNull(nameof(credentials));
            documentClient.ThrowIfNull(nameof(documentClient));

            this.credentials = credentials;
            this.documentClient = documentClient;
        }

        /// <summary>
        /// Handles the events that are posted.
        /// </summary>
        /// <param name="event">The posted @event</param>
        /// <returns>Action results.</returns>
        public async Task<HttpResponseMessage> Post(Event @event)
        {
            try
            {
                var querySpec = new SqlQuerySpec
                {
                    QueryText =
                        "SELECT * FROM subscriptions s WHERE s.profileDisplayName = @profileDisplayName AND s.subscriptionType = @subscriptionType",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@profileDisplayName", @event.Resource.Approval.Approver.DisplayName),
                        new SqlParameter("@subscriptionType", SubscriptionType.MyApprovals)
                    }
                };

                var subscriptions = this.documentClient
                    .CreateDocumentQuery<Subscription>(
                        UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList();

                foreach (var subscription in subscriptions)
                {
                    this.Notify(@event, subscription);
                }

                await Task.CompletedTask;

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private void Notify(Event @event, Subscription subscription)
        {
            try
            {
                MicrosoftAppCredentials.TrustServiceUrl(subscription.ServiceUri.AbsoluteUri);
                var client = new ConnectorClient(subscription.ServiceUri, this.credentials.MicrosoftAppId, this.credentials.MicrosoftAppPassword);

                var conversation =
                    client.Conversations.CreateDirectConversation(
                        new ChannelAccount(subscription.BotId, subscription.BotName),
                        new ChannelAccount(subscription.RecipientId, subscription.RecipientName));

                var activity = new Activity
                {
                    Conversation = new ConversationAccount
                    {
                        Id = conversation.Id
                    },
                    From = new ChannelAccount(subscription.BotId, subscription.BotName),
                    Recipient = new ChannelAccount(subscription.RecipientId, subscription.RecipientName),
                    Text = "Gogo approval",
                    Type = ActivityTypes.Message
                };

                client.Conversations.SendToConversation(activity, conversation.Id);
            }
            catch (Exception ex)
            {
                var x = ex.ToString();
            }
        }
    }
}