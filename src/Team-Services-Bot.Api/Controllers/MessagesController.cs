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
    using Autofac.Core;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the <see cref="ApiController"/> that handles incoming messages from the bot connector.
    /// </summary>
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private readonly IComponentContext container;
        private readonly IDialogInvoker dialogInvoker;
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagesController"/> class.
        /// </summary>
        /// <param name="container">A <see cref="IComponentContext"/>.</param>
        /// <param name="dialogInvoker">A <see cref="IDialogInvoker"/>.</param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/>.</param>
        public MessagesController(IComponentContext container, IDialogInvoker dialogInvoker, TelemetryClient telemetryClient)
        {
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
                    var dialog = this.container.Resolve<RootDialog>(new NamedParameter("eulaUri", new Uri($"{this.Request.RequestUri.GetLeftPart(UriPartial.Authority)}/Eula")));
                    await this.dialogInvoker.SendAsync(activity, () => dialog);
                }
                else
                {
                    this.HandleSystemMessage(activity);
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
        private void HandleSystemMessage(Activity message)
        {
            if (string.Compare(message.Type, ActivityTypes.DeleteUserData, StringComparison.OrdinalIgnoreCase) == 0)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
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
                // Handle knowing tha the user is typing
            }
            else if (string.Compare(message.Type, ActivityTypes.Ping, StringComparison.OrdinalIgnoreCase) == 0)
            {
            }
        }
    }
}