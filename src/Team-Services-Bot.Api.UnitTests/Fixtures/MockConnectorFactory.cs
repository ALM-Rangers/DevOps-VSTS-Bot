// ———————————————————————————————
// <copyright file="MockConnectorFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains a mocked implementation of IConnectorClientFactory.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using Microsoft.Rest;
    using Moq;

    /// <summary>
    /// An <see cref="IConnectorClientFactory"/> implementation that mocks the Connector Client.
    /// </summary>
    public class MockConnectorFactory : IConnectorClientFactory, IDisposable
    {
        private readonly IBotDataStore<BotData> memoryDataStore = new InMemoryDataStore();
        private readonly string botId;
        private StateClient stateClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockConnectorFactory"/> class.
        /// </summary>
        /// <param name="botId">The bot id.</param>
        public MockConnectorFactory(string botId)
        {
            SetField.NotNull(out this.botId, nameof(botId), botId);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="MockConnectorFactory"/> class.
        /// </summary>
        ~MockConnectorFactory()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Mocks a Stateclient.
        /// </summary>
        /// <param name="mockConnectorFactory">A <see cref="MockConnectorFactory"/>.</param>
        /// <returns>A mocked version of StateClient.</returns>
        public static Mock<StateClient> MockIBots(MockConnectorFactory mockConnectorFactory)
        {
            var botsClient = new Mock<StateClient>(MockBehavior.Loose);

            SetConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            SetUserDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetUserDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            SetPrivateConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetPrivateConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            return botsClient;
        }

        /// <inheritdoc />
        public IConnectorClient MakeConnectorClient()
        {
            var client = new Mock<ConnectorClient> { CallBase = true };
            return client.Object;
        }

        /// <inheritdoc />
        public IStateClient MakeStateClient()
        {
            return this.stateClient ?? (this.stateClient = MockIBots(this).Object);
        }

        /// <summary>
        /// Disposes the <see cref="MockConnectorFactory"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the <see cref="MockConnectorFactory"/>.
        /// </summary>
        /// <param name="disposing">A boolean value that indicates it is disposing or deconstructing the object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && this.stateClient != null)
            {
                this.stateClient.Dispose();
            }
        }

        /// <summary>
        /// Creates a default Address.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="conversationId">The conversation id.</param>
        /// <returns>Am <see cref="IAddress"/>.</returns>
        protected IAddress AddressFrom(string channelId, string userId, string conversationId)
        {
            var address = new Address(
                this.botId,
                channelId,
                userId ?? "AllUsers",
                conversationId ?? "AllConversations",
                "InvalidServiceUrl");

            return address;
        }

        /// <summary>
        /// Creates an <see cref="HttpOperationResponse"/>.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="conversationId">The conversation id.</param>
        /// <param name="storeType">The story type.</param>
        /// <param name="data">The data.</param>
        /// <returns>A <see cref="HttpOperationResponse"/>.</returns>
        protected async Task<HttpOperationResponse<object>> UpdateAndInsertData(string channelId, string userId, string conversationId, BotStoreType storeType, BotData data)
        {
            var result = new HttpOperationResponse<object> { Request = new HttpRequestMessage() };
            try
            {
                var address = this.AddressFrom(channelId, userId, conversationId);
                await this.memoryDataStore.SaveAsync(address, storeType, data, CancellationToken.None);
            }
            catch (HttpException e)
            {
                result.Body = e.Data;
                result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.PreconditionFailed };
                return result;
            }
            catch (Exception)
            {
                result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.InternalServerError };
                return result;
            }

            result.Body = data;
            result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
            return result;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="channelId">The channel id.</param>
        /// <param name="userId">The user id.</param>
        /// <param name="conversationId">The conversation id.</param>
        /// <param name="storeType">The story type.</param>
        /// <returns>A <see cref="HttpOperationResponse"/>.</returns>
        protected async Task<HttpOperationResponse<object>> GetData(string channelId, string userId, string conversationId, BotStoreType storeType)
        {
            var address = this.AddressFrom(channelId, userId, conversationId);
            var result = new HttpOperationResponse<object>
            {
                Request = new HttpRequestMessage(),
                Body = await this.memoryDataStore.LoadAsync(address, storeType, CancellationToken.None),
                Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK }
            };

            return result;
        }

        private static void GetPrivateConversationDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.GetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, userId, headers, token) =>
                {
                    return await mockConnectorFactory.GetData(channelId, userId, conversationId, BotStoreType.BotPrivateConversationData);
                });
        }

        private static void SetPrivateConversationDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.SetPrivateConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, userId, data, headers, token) =>
                {
                    return await mockConnectorFactory.UpdateAndInsertData(channelId, userId, conversationId, BotStoreType.BotPrivateConversationData, data);
                });
        }

        private static void GetUserDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.GetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, userId, headers, token) =>
                {
                    return await mockConnectorFactory.GetData(channelId, userId, null, BotStoreType.BotUserData);
                });
        }

        private static void SetUserDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.SetUserDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, userId, data, headers, token) =>
                {
                    return await mockConnectorFactory.UpdateAndInsertData(channelId, userId, null, BotStoreType.BotUserData, data);
                });
        }

        private static void GetConversationDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.GetConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, headers, token) =>
                {
                    return await mockConnectorFactory.GetData(channelId, null, conversationId, BotStoreType.BotConversationData);
                });
        }

        private static void SetConversationDataWithHttpMessagesAsync(MockConnectorFactory mockConnectorFactory, Mock<StateClient> botsClient)
        {
            botsClient
                .Setup(d => d.BotState.SetConversationDataWithHttpMessagesAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<BotData>(), It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<CancellationToken>()))
                .Returns<string, string, BotData, Dictionary<string, List<string>>, CancellationToken>(async (channelId, conversationId, data, headers, token) =>
                {
                    return await mockConnectorFactory.UpdateAndInsertData(channelId, null, conversationId, BotStoreType.BotConversationData, data);
                });
        }
    }
}