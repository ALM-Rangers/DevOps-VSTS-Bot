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
            var status = HttpStatusCode.OK;

            try
            {
                if (string.Equals(activity.Type, ActivityTypes.Message, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(activity.Type, ActivityTypes.ConversationUpdate, StringComparison.OrdinalIgnoreCase))
                {
                    RootDialog Dialog() => this.container.Resolve<RootDialog>(new NamedParameter("licenseUri", new Uri($"{this.Request.RequestUri.GetLeftPart(UriPartial.Authority)}/License")));
                    await Conversation.SendAsync(activity, Dialog);
                }
            }
            catch (Exception ex)
            {
                this.telemetryClient.TrackException(ex);
                status = HttpStatusCode.InternalServerError;
            }

            return this.Request.CreateResponse(status);
        }
    }
}