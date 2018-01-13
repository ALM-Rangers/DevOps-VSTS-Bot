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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Handles events from the VSTS Service hooks.
    /// </summary>
    public class EventController : ApiController
    {
        private readonly IBotDataFactory botDataFactory;
        private readonly MicrosoftAppCredentials credentials;
        private readonly IDocumentClient documentClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="botDataFactory">The bot data factory;</param>
        /// <param name="credentials">The Microsoft Application Credentials.</param>
        /// <param name="documentClient">The document client.</param>
        public EventController(IBotDataFactory botDataFactory, MicrosoftAppCredentials credentials, IDocumentClient documentClient)
        {
            credentials.ThrowIfNull(nameof(credentials));
            documentClient.ThrowIfNull(nameof(documentClient));

            this.botDataFactory = botDataFactory ?? throw new ArgumentNullException(nameof(botDataFactory));
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
                        "SELECT * FROM subscriptions s WHERE s.identityId = @identityId AND s.subscriptionType = @subscriptionType",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@identityId", @event.Resource.Approval.Approver.Id),
                        new SqlParameter("@subscriptionType", SubscriptionType.MyApprovals)
                    }
                };

                var subscriptions = this.documentClient
                    .CreateDocumentQuery<Subscription>(
                        UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList();

                var tasks = new List<Task>();

                foreach (var subscription in subscriptions)
                {
                    var t = this.Notify(@event, subscription);
                    tasks.Add(t);
                }

                await Task.WhenAll(tasks);

                return this.Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return this.Request.CreateResponse(HttpStatusCode.BadRequest, ex);
            }
        }

        private async Task Notify(Event @event, Subscription subscription)
        {
            try
            {
                var address = new Address(string.Empty, subscription.ChannelId, subscription.UserId, string.Empty, string.Empty);
                var botData = this.botDataFactory.Create(address);
                await botData.LoadAsync(CancellationToken.None);

                var data = botData.UserData.GetValue<UserData>("userData");

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
                    Text = Labels.PendingApproval,
                    Type = ActivityTypes.Message
                };

                var card = new ApprovalCard(data.Account, @event.Resource.Approval, data.TeamProject);
                activity.Attachments.Add(card);

                client.Conversations.SendToConversation(activity, conversation.Id);
            }
            catch (Exception ex)
            {
                var x = ex.ToString();
            }
        }
    }
}