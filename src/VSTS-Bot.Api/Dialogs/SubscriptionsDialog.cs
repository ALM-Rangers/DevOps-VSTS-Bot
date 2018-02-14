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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Teams;
    using Resources;
    using Strategies.Subscriptions;

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

        [NonSerialized]
        private IDocumentClient documentClient;

        [NonSerialized]
        private IEnumerable<ISubscriptionStrategy> strategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="documentClient">The document client.</param>
        /// <param name="strategies">The subscription strategies.</param>
        /// <param name="vstsService">VSTS accessor.</param>
        public SubscriptionsDialog(IAuthenticationService authenticationService, IDocumentClient documentClient, IEnumerable<ISubscriptionStrategy> strategies, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
            documentClient.ThrowIfNull(nameof(documentClient));
            strategies.ThrowIfNull(nameof(strategies));

            this.documentClient = documentClient;
            this.strategies = strategies;
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

                var querySpec = new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM subscriptions s WHERE s.channelId = @channelId AND s.userId = @userId",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@channelId", activity.ChannelId),
                        new SqlParameter("@userId", activity.From.Id)
                    }
                };

                var storedSubs = this.documentClient
                    .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList();

                var subscriptions = Enum
                    .GetValues(typeof(SubscriptionType))
                    .Cast<SubscriptionType>()
                    .Where(e => storedSubs.All(s => s.SubscriptionType != e))
                    .Select(e => new Subscription { SubscriptionType = e, ChannelId = activity.ChannelId, ProfileId = this.Profile.Id, UserId = activity.From.Id });

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
            var text = (activity.RemoveRecipientMention() ?? string.Empty).ToLowerInvariant().Trim();
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
                var subscriptionTypeTitle = string.Format(
                    CultureInfo.CurrentCulture,
                    Labels.ResourceManager.GetString("SubscriptionTitle_" + subscriptionType));
                var querySpec = new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM subscriptions s WHERE s.channelId = @channelId AND s.userId = @userId AND s.subscriptionType = @subscriptionType",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@channelId", activity.ChannelId),
                        new SqlParameter("@userId", activity.From.Id),
                        new SqlParameter("@subscriptionType", subscriptionType)
                    }
                };

                var subscription = this.documentClient
                    .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList()
                    .FirstOrDefault();

                if (subscription != null)
                {
                    context.Done(reply);
                    return;
                }

                var teamProjects = await this.VstsService.GetProjects(this.Account, this.Profile.Token);
                var teamProject = teamProjects.FirstOrDefault(tp => tp.Name.Equals(this.TeamProject, StringComparison.OrdinalIgnoreCase));

                if (teamProject == null)
                {
                    context.Done(reply);
                    return;
                }

                subscription = new Subscription
                {
                    BotId = activity.Recipient.Id,
                    BotName = activity.Recipient.Name,
                    ChannelId = activity.ChannelId,
                    IsActive = true,
                    ProfileId = this.Profile.Id,
                    RecipientId = activity.From.Id,
                    RecipientName = activity.From.Name,
                    ServiceUri = new Uri(activity.ServiceUrl),
                    SubscriptionType = subscriptionType,
                    TenantId = activity.ChannelId.Equals(ChannelIds.Msteams) ? activity.GetTenantId() : string.Empty,
                    UserId = activity.From.Id
                };

                var strategy = this.strategies.First(s => s.CanGetSubscription(subscriptionType));
                var s2 = strategy.GetSubscription(subscription.Id, teamProject);

                var r = await this.VstsService.CreateSubscription(this.Account, s2, this.Profile.Token);

                subscription.IdentityId = r.CreatedBy.Id;
                subscription.SubscriptionId = r.Id;

                await this.documentClient.UpsertDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), subscription);

                reply.Text = string.Format(Labels.Subscribed, subscriptionTypeTitle);

                await context.PostAsync(reply);
                context.Done(reply);
            }
            else if (matchUnsubscribe.Success)
            {
                var subscriptionType = (SubscriptionType)Enum.Parse(typeof(SubscriptionType), matchUnsubscribe.Groups[1].Value, true);
                var subscriptionTypeTitle = string.Format(
                    CultureInfo.CurrentCulture,
                    Labels.ResourceManager.GetString("SubscriptionTitle_" + subscriptionType));
                var querySpec = new SqlQuerySpec
                {
                    QueryText = "SELECT * FROM subscriptions s WHERE s.channelId = @channelId AND s.userId = @userId AND s.subscriptionType = @subscriptionType",
                    Parameters = new SqlParameterCollection
                    {
                        new SqlParameter("@channelId", activity.ChannelId),
                        new SqlParameter("@userId", activity.From.Id),
                        new SqlParameter("@subscriptionType", subscriptionType)
                    }
                };

                var subscription = this.documentClient
                    .CreateDocumentQuery<Subscription>(UriFactory.CreateDocumentCollectionUri("botdb", "subscriptioncollection"), querySpec)
                    .ToList()
                    .FirstOrDefault();

                if (subscription != null)
                {
                    await this.documentClient.DeleteDocumentAsync(UriFactory.CreateDocumentUri("botdb", "subscriptioncollection", subscription.Id.ToString()));

                    await this.VstsService.DeleteSubscription(this.Account, subscription.SubscriptionId, this.Profile.Token);

                    reply.Text = string.Format(Labels.Unsubscribed, subscriptionTypeTitle);
                    await context.PostAsync(reply);
                }

                context.Done(reply);
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.documentClient = GlobalConfiguration.Configuration.DependencyResolver.GetService<IDocumentClient>();
            this.strategies = GlobalConfiguration.Configuration.DependencyResolver.GetService<IEnumerable<ISubscriptionStrategy>>();
        }
    }
}