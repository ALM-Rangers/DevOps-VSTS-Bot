//———————————————————————————————
// <copyright file=”name of this file, i.e. MockConnectorFactory.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains a mocked implementation of IConnectorClientFactory.
// </summary>
//———————————————————————————————

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


namespace Vsar.TSBot.UnitTests
{
    public class MockConnectorFactory : IConnectorClientFactory, IDisposable
    {
        private readonly IBotDataStore<BotData> _memoryDataStore = new InMemoryDataStore();
        private readonly string _botId;
        private StateClient _stateClient;

        public MockConnectorFactory(string botId)
        {
            SetField.NotNull(out _botId, nameof(botId), botId);
        }

        public IConnectorClient MakeConnectorClient()
        {
            var client = new Mock<ConnectorClient> {CallBase = true};
            return client.Object;
        }

        public IStateClient MakeStateClient()
        {
            return _stateClient ?? (_stateClient = MockIBots(this).Object);
        }

        protected IAddress AddressFrom(string channelId, string userId, string conversationId)
        {
            var address = new Address
            (
                _botId,
                channelId,
                userId ?? "AllUsers",
                conversationId ?? "AllConversations",
                "InvalidServiceUrl"
            );
            return address;
        }
        protected async Task<HttpOperationResponse<object>> UpdateAndInsertData(string channelId, string userId, string conversationId, BotStoreType storeType, BotData data)
        {
            var result = new HttpOperationResponse<object> {Request = new HttpRequestMessage()};
            try
            {
                var address = AddressFrom(channelId, userId, conversationId);
                await _memoryDataStore.SaveAsync(address, storeType, data, CancellationToken.None);
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

        protected async Task<HttpOperationResponse<object>> GetData(string channelId, string userId, string conversationId, BotStoreType storeType)
        {
            var address = AddressFrom(channelId, userId, conversationId);
            var result = new HttpOperationResponse<object> {Request = new HttpRequestMessage()};
            result.Body = await _memoryDataStore.LoadAsync(address, storeType, CancellationToken.None);
            result.Response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK };

            return result;
        }

        public static Mock<StateClient> MockIBots(MockConnectorFactory mockConnectorFactory)
        {
            var botsClient = new Moq.Mock<StateClient>(MockBehavior.Loose);

            SetConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            SetUserDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetUserDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            SetPrivateConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            GetPrivateConversationDataWithHttpMessagesAsync(mockConnectorFactory, botsClient);

            return botsClient;
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && _stateClient != null)
            {
                _stateClient.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MockConnectorFactory()
        {
            Dispose(false);
        }
    }
}