// ———————————————————————————————
// <copyright file="BotService.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an implementation for IBotService.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents an implementation for IBotService.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class BotService : IBotService
    {
        private readonly IBotDataStore<BotData> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        public BotService(IBotDataStore<BotData> store)
        {
            this.store = store;
        }

        /// <inheritdoc />
        public async Task<BotData> GetUserData(string channelId, string userId)
        {
            var key = new Address(string.Empty, channelId, userId, string.Empty, string.Empty);
            return await this.store.LoadAsync(key, BotStoreType.BotUserData, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task SetUserData(string channelId, string userId, BotData botData)
        {
            var key = new Address(string.Empty, channelId, userId, string.Empty, string.Empty);
            await this.store.SaveAsync(key, BotStoreType.BotUserData, botData, CancellationToken.None);
            await this.store.FlushAsync(key, CancellationToken.None);
        }
    }
}