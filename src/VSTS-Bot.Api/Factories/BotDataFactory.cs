// ———————————————————————————————
// <copyright file="BotDataFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Instantiates an IBotData.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Instantiates an <see cref="IBotData"/>.
    /// </summary>
    public class BotDataFactory : IBotDataFactory
    {
        private readonly IBotDataStore<BotData> store;

        /// <summary>
        /// Initializes a new instance of the <see cref="BotDataFactory"/> class.
        /// </summary>
        /// <param name="store">The <see cref="IBotDataStore{T}"/></param>
        public BotDataFactory(IBotDataStore<BotData> store)
        {
            store.ThrowIfNull(nameof(store));

            this.store = store;
        }

        /// <inheritdoc />
        public IBotData Create(Address address)
        {
            address.ThrowIfNull(nameof(address));

            return new JObjectBotData(address, this.store);
        }
    }
}