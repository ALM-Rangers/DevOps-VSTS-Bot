using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;

namespace Vsar.TSBot.UnitTests
{
    public class DialogFixture
    {
        public const string User = "testUser";

        public const string Bot = "testBot";

        public IMessageActivity CreateMessage()
        {
            return new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = ActivityTypes.Message,
                From = new ChannelAccount { Id = User },
                Conversation = new ConversationAccount { Id = Guid.NewGuid().ToString() },
                Recipient = new ChannelAccount { Id = Bot },
                ServiceUrl = "InvalidServiceUrl",
                ChannelId = "Test",
                Attachments = Array.Empty<Attachment>(),
                Entities = Array.Empty<Entity>(),
            };
        }

        public IContainer Build(params object[] singletons)
        {
            if(singletons == null)
                throw new ArgumentNullException(nameof(singletons));

            var builder = new ContainerBuilder();
            builder
                .RegisterModule(new DialogModule_MakeRoot());

            builder
                .Register((c, p) => new MockConnectorFactory(c.Resolve<IAddress>().BotId))
                .As<IConnectorClientFactory>()
                .InstancePerLifetimeScope();

            builder
                .Register(c => new Queue<IMessageActivity>())
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .RegisterType<BotToUserQueue>()
                .AsSelf()
                .InstancePerLifetimeScope();

            builder
                .Register(c => new MapToChannelData_BotToUser(c.Resolve<BotToUserQueue>(), new List<IMessageActivityMapper> { new KeyboardCardMapper() }))
                .As<IBotToUser>()
                .InstancePerLifetimeScope();

            foreach (var singleton in singletons)
            {
                builder
                    .Register(c => singleton)
                    .Keyed(FiberModule.Key_DoNotSerialize, singleton.GetType());
            }

            return builder.Build();
        }

        public async Task<IMessageActivity> GetResponse(IContainer container, IDialog<object> root, IMessageActivity toBot)
        {
            using (var scope = DialogModule.BeginLifetimeScope(container, toBot))
            {
                DialogModule_MakeRoot.Register(scope, () => root);

                // act: sending the message
                var task = scope.Resolve<IPostToBot>();
                await task.PostAsync(toBot, default(CancellationToken));

                return scope.Resolve<Queue<IMessageActivity>>().Dequeue();
            }
        }
    }
}