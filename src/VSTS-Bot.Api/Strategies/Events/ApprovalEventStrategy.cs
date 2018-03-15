// ———————————————————————————————
// <copyright file="ApprovalEventStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Implementation of a strategy for handling events for approvals.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Strategies.Events
{
    using System.Threading;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;
    using TSBot.Events;
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
        public async Task Handle(ServiceHookEventBase serviceHookEvent, Subscription subscription)
        {
            var ev = serviceHookEvent as ServiceHookEvent<ApprovalResource>;

            ev.ThrowIfNull(nameof(serviceHookEvent));
            subscription.ThrowIfNull(nameof(subscription));

            var address = new Address(string.Empty, subscription.ChannelId, subscription.UserId, string.Empty, string.Empty);
            var botData = this.botDataFactory.Create(address);
            await botData.LoadAsync(CancellationToken.None);

            var data = botData.UserData.GetValue<UserData>("userData");

            MicrosoftAppCredentials.TrustServiceUrl(subscription.ServiceUri.AbsoluteUri);
            var client = new ConnectorClient(subscription.ServiceUri, this.credentials.MicrosoftAppId, this.credentials.MicrosoftAppPassword);

            var conversation = subscription.ChannelId.Equals(ChannelIds.Msteams)
                ? client.Conversations.CreateOrGetDirectConversation(
                    new ChannelAccount(subscription.BotId, subscription.BotName),
                    new ChannelAccount(subscription.RecipientId, string.Empty),
                    subscription.TenantId)
                : client.Conversations.CreateDirectConversation(
                    new ChannelAccount(subscription.BotId, subscription.BotName),
                    new ChannelAccount(subscription.RecipientId, string.Empty));

            var activity = new Activity
            {
                Conversation = new ConversationAccount
                {
                    Id = conversation.Id
                },
                From = new ChannelAccount(subscription.BotId, subscription.BotName),
                Recipient = new ChannelAccount(subscription.RecipientId, string.Empty),
                Text = Labels.PendingApproval,
                Type = ActivityTypes.Message
            };

            var card = new ApprovalCard(data.Account, ev.Resource.Approval, data.TeamProject);
            activity.Attachments.Add(card);

            client.Conversations.SendToConversation(activity, conversation.Id);
        }

        /// <inheritdoc />
        public bool ShouldHandle(ServiceHookEventBase serviceHookEvent)
        {
            serviceHookEvent.ThrowIfNull(nameof(serviceHookEvent));

            return serviceHookEvent.GetType() == typeof(ServiceHookEvent<ApprovalResource>);
        }
    }
}