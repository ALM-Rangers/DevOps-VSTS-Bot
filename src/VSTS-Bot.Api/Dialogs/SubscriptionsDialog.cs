// ———————————————————————————————
// <copyright file="SubscriptionsDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a dialog for subscriptions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents a dialog for subscriptions.
    /// </summary>
    [CommandMetadata("subscriptions")]
    [Serializable]
    public class SubscriptionsDialog : DialogBase, IDialog<object>
    {
        private const string CommandMatchSubscriptions = "subscriptions";
        private const string CommandMatchSubscribe = @"^subscribe (.+)";
        private const string CommandMatchUnsubscribe = @"^unsubscribe (.+)";

        private readonly IDocumentClient documentClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="documentClient">The document client.</param>
        /// <param name="vstsService">VSTS accessor.</param>
        public SubscriptionsDialog(IAuthenticationService authenticationService, IDocumentClient documentClient, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
            documentClient.ThrowIfNull(nameof(documentClient));

            this.documentClient = documentClient;
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets or sets the Team Project.
        /// </summary>
        public string TeamProject { get; set; }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.ThrowIfNull(nameof(context));

            context.Wait(this.SubscriptionsAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of subscriptions for the user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SubscriptionsAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();

            if (text.Equals(CommandMatchSubscriptions, StringComparison.OrdinalIgnoreCase))
            {
                var data = context.UserData.GetValue<UserData>("userData");

                this.Account = data.Account;
                this.Profile = await this.GetValidatedProfile(context.UserData);
                this.TeamProject = data.TeamProject;

                var typing = context.MakeMessage();
                typing.Type = ActivityTypes.Typing;
                await context.PostAsync(typing);

                var storedSubs = this.documentClient
                    .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"))
                    .Where(s => string.Equals(s.ChannelId, activity.ChannelId, StringComparison.Ordinal) &&
                                string.Equals(s.UserId, activity.From.Id, StringComparison.Ordinal))
                    .OrderBy(s => s.SubscriptionType)
                    .ToList();

                var subscriptions = Enum
                    .GetValues(typeof(SubscriptionType))
                    .Cast<SubscriptionType>()
                    .Where(e => storedSubs.All(s => s.SubscriptionType != e))
                    .Select(e => new Subscription { SubscriptionType = e,  ChannelId = activity.ChannelId, UserId = activity.From.Id });

                var cards = storedSubs
                    .Union(subscriptions)
                    .OrderBy(s => s.SubscriptionType)
                    .Select(s => new SubscriptionCard(s, this.TeamProject))
                    .ToList();

                var reply = context.MakeMessage();
                foreach (var card in cards)
                {
                    reply.Attachments.Add(card);
                }

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                await context.PostAsync(reply);
                context.Wait(this.SubscribeAsync);
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }

        /// <summary>
        /// Subscribes to a subscribtion.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task SubscribeAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;
            var text = (activity.RemoveRecipientMention() ?? string.Empty).ToLowerInvariant();
            var reply = context.MakeMessage();

            var matchSubscribe = Regex.Match(text, CommandMatchSubscribe);
            var matchUnsubscribe = Regex.Match(text, CommandMatchUnsubscribe);

            if (matchSubscribe.Success || matchUnsubscribe.Success)
            {
                var typing = context.MakeMessage();
                typing.Type = ActivityTypes.Typing;
                await context.PostAsync(typing);
            }

            if (matchSubscribe.Success)
            {
                var subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), matchSubscribe.Groups[1].Value, true);
                var subscription = this.documentClient
                                       .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"))
                                       .Where(s => string.Equals(s.ChannelId, activity.ChannelId, StringComparison.Ordinal) && string.Equals(s.UserId, activity.From.Id, StringComparison.Ordinal))
                                       .FirstOrDefault(s => s.SubscriptionType == subscriptionType) ??
                                   new Subscription
                                   {
                                       ChannelId = activity.ChannelId,
                                       UserId = activity.From.Id,
                                       SubscriptionType = subscriptionType,
                                       IsActive = true
                                   };
                await this.documentClient.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), subscription);

                reply.Text = Labels.Subscribed;

                await context.PostAsync(reply);
                context.Done(reply);
            }
            else if (matchUnsubscribe.Success)
            {
                var subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), matchUnsubscribe.Groups[1].Value, true);
                var subscription = this.documentClient
                                       .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"))
                                       .Where(s => string.Equals(s.ChannelId, activity.ChannelId, StringComparison.Ordinal) && string.Equals(s.UserId, activity.From.Id, StringComparison.Ordinal))
                                       .FirstOrDefault(s => s.SubscriptionType == subscriptionType);

                if (subscription != null)
                {
                    await this.documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri("botdb", "subscriptioncollection", subscription.Id.ToString()));

                    reply.Text = Labels.Unsubscribed;
                    await context.PostAsync(reply);
                }

                context.Done(reply);
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }
    }
}