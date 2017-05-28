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
    using System.Web.Http;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Moq;

    /// <summary>
    /// A fixture for dialogs.
    /// </summary>
    public class DialogFixture : IDisposable
    {
        private const string Bot = "testBot";
        private const string User = "testUser";

        public DialogFixture()
        {
            this.AuthenticationService = new Mock<IAuthenticationService>();
            this.RootDialog = new RootDialog(this.AuthenticationService.Object, this.TelemetryClient);
            this.DialogContext
                .Setup(c => c.UserData)
                .Returns(this.UserData.Object);
            this.DialogContext
                .Setup(c => c.MakeMessage())
                .Returns(this.CreateMessage);
        }

        public Mock<IAuthenticationService> AuthenticationService { get; }

        public Mock<IDialogContext> DialogContext { get; } = new Mock<IDialogContext>();

        /// <summary>
        /// Gets the root dialog.
        /// </summary>
        public RootDialog RootDialog { get; }

        public TelemetryClient TelemetryClient { get; } = new TelemetryClient();

        /// <summary>
        /// Gets a mocked user data.
        /// </summary>
        public Mock<IBotDataBag> UserData { get; } = new Mock<IBotDataBag>();

        /// <summary>
        /// Gets mocked <see cref="IVstsService"/>
        /// </summary>
        public Mock<IVstsService> VstsService { get; } = new Mock<IVstsService>();

        /// <summary>
        /// Creates a default <see cref="IMessageActivity"/>.
        /// </summary>
        /// <returns>A <see cref="IMessageActivity"/>.</returns>
        public Activity CreateMessage()
        {
            return new Activity
            {
                Id = Guid.NewGuid().ToString(),
                Type = ActivityTypes.Message,
                From = new ChannelAccount { Id = User, Name = User },
                Conversation = new ConversationAccount { Id = Guid.NewGuid().ToString() },
                Recipient = new ChannelAccount { Id = Bot },
                ServiceUrl = "InvalidServiceUrl",
                ChannelId = "Test",
                Attachments = new List<Attachment>(),
                Entities = new List<Entity>(),
            };
        }

        public VstsProfile CreateProfile()
        {
            return new VstsProfile
            {
                Token = new OAuthToken { ExpiresIn = 3600 }
            };
        }

        /// <summary>
        /// Instantiates a <see cref="ContainerBuilder"/> and registers some defaults.
        /// </summary>
        /// <param name="builder">A container builder.</param>
        /// <returns>A <see cref="ContainerBuilder"/>.</returns>
        public IContainer Build(ContainerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder
                .Register(c => this.AuthenticationService.Object)
                .As<IAuthenticationService>();

            builder
                .Register(c => this.VstsService.Object)
                .As<IVstsService>();

            builder
                .Register(c => this.RootDialog);

            builder
                .RegisterModule<AttributedMetadataModule>();

            builder
                .RegisterType<RootDialog>();

            builder
                .RegisterModule(new DialogModule_MakeRoot());

            builder
                .Register((c, p) => new MockConnectorFactory(c.Resolve<IAddress>().BotId))
                .As<IConnectorClientFactory>().InstancePerLifetimeScope();

            builder.Register(c => new Queue<IMessageActivity>())
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

            var container = builder.Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            return container;
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

        public IAwaitable<T> MakeAwaitable<T>(T item)
        {
            return new AwaitableFromItem<T>(item);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposable)
        {
            if (disposable)
            {
                // Managed code.
            }
        }
    }
}