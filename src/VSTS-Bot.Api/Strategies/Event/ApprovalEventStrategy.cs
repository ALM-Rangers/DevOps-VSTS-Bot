// ———————————————————————————————
// <copyright file="ApprovalEventStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Implementation of a strategy for handling events for approvals.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Strategies.Event
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cards;
    using Events;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;
    using Subscription = TSBot.Subscription;

    /// <summary>
    /// Implementation of a strategy for handling events for approvals.
    /// </summary>
    public class ApprovalEventStrategy : IEventStrategy
    {
        private readonly IBotDataFactory botDataFactory;
        private readonly MicrosoftAppCredentials credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalEventStrategy"/> class.
        /// </summary>
        /// <param name="botDataFactory">The bot data factory.</param>
        /// <param name="credentials">The credentials.</param>
        public ApprovalEventStrategy(IBotDataFactory botDataFactory, MicrosoftAppCredentials credentials)
        {
            botDataFactory.ThrowIfNull(nameof(botDataFactory));
            credentials.ThrowIfNull(nameof(credentials));

            this.botDataFactory = botDataFactory;
            this.credentials = credentials;
        }

        /// <inheritdoc />
        public async Task Handle(EventBase @event, Subscription subscription)
        {
            var ev = @event as Event<ApprovalResource>;

            ev.ThrowIfNull(nameof(@event));
            subscription.ThrowIfNull(nameof(subscription));

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

            var card = new ApprovalCard(data.Account, ev.Resource.Approval, data.TeamProject);
            activity.Attachments.Add(card);

            client.Conversations.SendToConversation(activity, conversation.Id);
        }

        /// <inheritdoc />
        public bool ShouldHandle(EventBase @event)
        {
            return @event.GetType() == typeof(Event<ApprovalResource>);
        }
    }
}