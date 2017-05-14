// ———————————————————————————————
// <copyright file="IBotService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for connecting services of the bot connector.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents an interface for connecting services of the bot connector.
    /// </summary>
    public interface IBotService
    {
        /// <summary>
        /// Gets the bot data.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The botdata.</returns>
        Task<BotData> GetUserData(string channelId, string userId);

        /// <summary>
        /// Sets the botdata.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="botData">The botdata.</param>
        /// <returns>Nothing.</returns>
        Task SetUserData(string channelId, string userId, BotData botData);
    }
}