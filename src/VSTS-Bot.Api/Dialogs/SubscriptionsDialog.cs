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
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents a dialog for subscriptions.
    /// </summary>
    [CommandMetadata("subscriptions")]
    [Serializable]
    public class SubscriptionsDialog : DialogBase, IDialog<object>
    {
        private const string CommandMatchSubscriptions = "subscriptions";

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionsDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">VSTS accessor.</param>
        public SubscriptionsDialog(IAuthenticationService authenticationService, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
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

                var cards = Enum
                    .GetValues(typeof(SubscriptionType))
                    .Cast<SubscriptionType>()
                    .Select(e => new SubscriptionCard(new Subscription { SubscriptionType = e }, this.TeamProject));

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
            await Task.CompletedTask;
        }
    }
}