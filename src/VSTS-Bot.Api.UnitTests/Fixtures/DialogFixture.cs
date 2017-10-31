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
    using System.Diagnostics.CodeAnalysis;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Moq;

    /// <summary>
    /// A fixture for dialogs.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class DialogFixture : IDisposable
    {
        private const string Bot = "testBot";
        private const string User = "testUser";

        public DialogFixture()
        {
            this.RootDialog = new RootDialog(new Uri("https://an.url.toEula"), this.TelemetryClient);
            this.DialogContext
                .Setup(c => c.UserData)
                .Returns(this.UserData.Object);
            this.DialogContext
                .Setup(c => c.MakeMessage())
                .Returns(this.CreateMessage);
        }

        /// <summary>
        /// Gets the authentication service.
        /// </summary>
        public Mock<IAuthenticationService> AuthenticationService { get; } = new Mock<IAuthenticationService>();

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

        public Profile CreateProfile()
        {
            return new Profile
            {
                IsSelected = true,
                IsValidated = true,
                Token = new OAuthToken { ExpiresIn = 3600 }
            };
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