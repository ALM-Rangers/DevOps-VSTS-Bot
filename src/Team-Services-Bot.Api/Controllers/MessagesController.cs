// ———————————————————————————————
// <copyright file="MessagesController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Controller logic to process messages from the Bot Connector.
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

    /// <summary>
    /// Represents the <see cref="ApiController"/> that handles incoming messages from the bot connector.
    /// </summary>
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IComponentContext container;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="container">A <see cref="IComponentContext"/>.</param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/>.</param>
        public MessagesController(IComponentContext container, TelemetryClient telemetryClient)
        {
            this.container = container;
            this.telemetryClient = telemetryClient;
        }

        /// <summary>
        /// Handles the Post requests.
        /// </summary>
        /// <param name="activity">The incoming <see cref="Activity"/>.</param>
        /// <returns>a <see cref="HttpResponseMessage"/>.</returns>
        public async Task<HttpResponseMessage> Post([FromBody] Activity activity)
        {
            try
            {
                var dialog = this.container.Resolve<RootDialog>();

                if (activity.Type == ActivityTypes.Message)
                {
                    await Conversation.SendAsync(activity, () => dialog);
                }
                else
                {
                    this.HandleSystemMessage(activity);
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
            }

            return this.Request.CreateResponse(HttpStatusCode.OK);
        }

        private void HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }
        }
    }
}