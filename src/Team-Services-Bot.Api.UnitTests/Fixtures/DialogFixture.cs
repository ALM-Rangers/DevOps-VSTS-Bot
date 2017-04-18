// ———————————————————————————————
// <copyright file="DialogFixture.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains a basic fixture for testing dialogs.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Vsar.TSBot.Dialogs;

    /// <summary>
    /// A fixture for dialogs.
    /// </summary>
    public class DialogFixture
    {
        private const string Bot = "testBot";
        private const string User = "testUser";

        /// <summary>
        /// Creates a default <see cref="IMessageActivity"/>.
        /// </summary>
        /// <returns>A <see cref="IMessageActivity"/>.</returns>
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

        /// <summary>
        /// Instantiates a <see cref="ContainerBuilder"/> and registers some defaults.
        /// </summary>
        /// <returns>A <see cref="ContainerBuilder"/>.</returns>
        public ContainerBuilder Build()
        {
            var builder = new ContainerBuilder();

            builder
                .RegisterModule<AttributedMetadataModule>();
            builder
                .RegisterType<RootDialog>();

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

            return builder;
        }

        /// <summary>
        /// Gets a response.
        /// </summary>
        /// <param name="container">A <see cref="IContainer"/>.</param>
        /// <param name="root">A <see cref="IDialog{TResult}"/> as root.</param>
        /// <param name="toBot">A <see cref="IMessageActivity"/> as the to bot message.</param>
        /// <returns>A <see cref="IMessageActivity"/> as response.</returns>
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