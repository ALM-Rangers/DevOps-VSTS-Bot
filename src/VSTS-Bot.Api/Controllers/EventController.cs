// ———————————————————————————————
// <copyright file="EventController.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Handles events from the VSTS Service hooks.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;

    /// <summary>
    /// Handles events from the VSTS Service hooks.
    /// </summary>
    public class EventController : ApiController
    {
        /// <summary>
        /// Handles the events that are posted.
        /// </summary>
        /// <param name="event">The posted @event</param>
        /// <returns>Action results.</returns>
        public async Task<HttpResponseMessage> Post(Event @event)
        {
            var status = HttpStatusCode.OK;

            await Task.CompletedTask;

            return this.Request.CreateResponse(status);
        }
    }
}