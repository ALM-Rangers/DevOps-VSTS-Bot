// ———————————————————————————————
// <copyright file="MessagesController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides the controller logic to process messages from the Bot Connector.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Newtonsoft.Json.Linq;
    using Resources;

    /// <summary>
    /// Represents the <see cref="ApiController"/> that handles incoming messages from the bot connector.
    /// </summary>
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IComponentContext container;
        private readonly IBotService botService;
        private readonly IDialogInvoker dialogInvoker;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="botService">The botService.</param>
        /// <param name="container">A <see cref="IComponentContext"/>.</param>
        /// <param name="dialogInvoker">A <see cref="IDialogInvoker"/>.</param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/>.</param>
        public MessagesController(IBotService botService, IComponentContext container, IDialogInvoker dialogInvoker, TelemetryClient telemetryClient)
        {
            this.botService = botService;
            this.container = container;
            this.dialogInvoker = dialogInvoker;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Handles the Post requests.
        /// </summary>
        /// <param name="activity">The incoming <see cref="Activity"/>.</param>
        /// <returns>a <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            var status = HttpStatusCode.OK;

            try
            {
                if (string.Equals(activity.Type, ActivityTypes.Message, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(activity.Type, ActivityTypes.ConversationUpdate, StringComparison.OrdinalIgnoreCase))
                {
                    dynamic data = activity.ChannelData;

                    if (activity.ChannelId.Equals(ChannelIds.Slack, StringComparison.OrdinalIgnoreCase) &&
                        data.SlackMessage.type == "event_callback")
                    {
                        return this.Request.CreateResponse(status);
                    }

                    var dialog = this.container.Resolve<RootDialog>(new NamedParameter("eulaUri", new Uri($"{this.Request.RequestUri.GetLeftPart(UriPartial.Authority)}/Eula")));
                    await this.dialogInvoker.SendAsync(activity, () => dialog);
                }
                else
                {
                    await this.HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                status = HttpStatusCode.InternalServerError;
            }

            return this.Request.CreateResponse(status);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Reviewed.")]
        private async Task HandleSystemMessage(Activity message)
        {
            if (string.Compare(message.Type, ActivityTypes.DeleteUserData, StringComparison.OrdinalIgnoreCase) == 0)
            {
                var data = await this.botService.GetUserData(message.ChannelId, message.From.Id);
                await this.botService.SetUserData(message.ChannelId, message.From.Id, new BotData(data.ETag));

                var reply = message.CreateReply(Labels.UserDataDeleted);
                var connector = new ConnectorClient(new Uri(message.ServiceUrl));
                connector.Conversations.ReplyToActivity(reply);
            }
            else if (string.Compare(message.Type, ActivityTypes.ConversationUpdate, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (string.Compare(message.Type, ActivityTypes.ContactRelationUpdate, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (string.Compare(message.Type, ActivityTypes.Typing, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Handle knowing that the user is typing
            }
            else if (string.Compare(message.Type, ActivityTypes.Ping, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Handle ping message
            }
        }
    }
}