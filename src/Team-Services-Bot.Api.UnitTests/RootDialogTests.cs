using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using FluentAssertions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vsar.TSBot.Dialogs;

namespace Team_Services_Bot.Api.UnitTests
{
    [Flags]
    public enum Options { None = 0, Reflection = 1, ScopedQueue = 2, MockConnectorFactory = 4, ResolveDialogFromContainer = 8, LastWriteWinsCachingBotDataStore = 16 };

    [TestClass]
    public class RootDialogTests
    {
        public static class ChannelId
        {
            public const string User = "testUser";
            public const string Bot = "testBot";
        }

        public static IMessageActivity MakeTestMessage()
        {
            return new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = ActivityTypes.Message,
                From = new ChannelAccount { Id = ChannelId.User },
                Conversation = new ConversationAccount { Id = Guid.NewGuid().ToString() },
                Recipient = new ChannelAccount { Id = ChannelId.Bot },
                ServiceUrl = "InvalidServiceUrl",
                ChannelId = "Test",
                Attachments = Array.Empty<Attachment>(),
                Entities = Array.Empty<Entity>(),
            };
        }

        public static IContainer Build(Options options, params object[] singletons)
        {
            var builder = new ContainerBuilder();
            if (options.HasFlag(Options.ResolveDialogFromContainer))
            {
                builder.RegisterModule(new DialogModule());
            }
            else
            {
                builder.RegisterModule(new DialogModule_MakeRoot());
            }

            // make a "singleton" MockConnectorFactory per unit test execution
            IConnectorClientFactory factory = null;
            builder
                .Register((c, p) => factory ?? (factory = new MockConnectorFactory(c.Resolve<IAddress>().BotId)))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            if (options.HasFlag(Options.Reflection))
            {
                builder.RegisterModule(new ReflectionSurrogateModule());
            }

            var r =
                builder
                .Register(c => new Queue<IMessageActivity>())
                .AsSelf();

            if (options.HasFlag(Options.ScopedQueue))
            {
                r.InstancePerLifetimeScope();
            }
            else
            {
                r.SingleInstance();
            }

            builder
                .RegisterType<BotToUserQueue>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .Register(c => new MapToChannelData_BotToUser(
                    c.Resolve<BotToUserQueue>(),
                    new List<IMessageActivityMapper> { new KeyboardCardMapper() }))
                .As<IBotToUser>()
                .InstancePerLifetimeScope();

            if (options.HasFlag(Options.LastWriteWinsCachingBotDataStore))
            {
                builder.Register(c => new CachingBotDataStore(c.ResolveKeyed<IBotDataStore<BotData>>(typeof(ConnectorStore)), CachingBotDataStoreConsistencyPolicy.LastWriteWins))
                    .As<IBotDataStore<BotData>>()
                    .AsSelf()
                    .InstancePerLifetimeScope();
            }

            foreach (var singleton in singletons)
            {
                builder
                    .Register(c => singleton)
                    .Keyed(FiberModule.Key_DoNotSerialize, singleton.GetType());
            }

            return builder.Build();
        }

        private async Task<IMessageActivity> GetResponse(IContainer container, Func<IDialog<object>> makeRoot, IMessageActivity toBot)
        {
            using (var scope = DialogModule.BeginLifetimeScope(container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, makeRoot);

                // act: sending the message
                var task = scope.Resolve<IPostToBot>();
                await task.PostAsync(toBot, default(CancellationToken));

                return scope.Resolve<Queue<IMessageActivity>>().Dequeue();
            }
        }

        [TestMethod]
        public async Task EchoDialog()
        {
            var toBot = MakeTestMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "Test";

            var dialog = new RootDialog();
            Func<IDialog<object>> makeRoot = () => dialog;

            using (var container = Build(Options.MockConnectorFactory | Options.ScopedQueue, dialog))
            {
                var toUser = await GetResponse(container, makeRoot, toBot);

                toUser.Text.Should().Contain("4").And.Contain("Test");
            }
        }
    }
}