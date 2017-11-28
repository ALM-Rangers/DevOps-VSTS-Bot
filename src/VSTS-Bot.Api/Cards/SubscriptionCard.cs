// ———————————————————————————————
// <copyright file="SubscriptionCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the card for a subscription.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Cards
{
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represets the card for a subscription.
    /// </summary>
    public class SubscriptionCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionCard"/> class.
        /// </summary>
        /// <param name="subscription">The subscription</param>
        /// <param name="teamProject">The team project.</param>
        public SubscriptionCard(Subscription subscription, string teamProject)
        {
            subscription.ThrowIfNull(nameof(subscription));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));

            this.Title = string.Format(Labels.ResourceManager.GetString("SubscriptionTitle_" + subscription.Type), teamProject);
            this.Subtitle = Labels.ResourceManager.GetString("SubscriptionDescription_" + subscription.Type);

            var action = subscription.IsActive
                ? new CardAction(ActionTypes.ImBack, Labels.Unsubscribe, value: $"unsubscribe {subscription.Type}")
                : new CardAction(ActionTypes.ImBack, Labels.Subscribe, value: $"subscribe {subscription.Type}");
            this.Buttons.Add(action);
        }
    }
}