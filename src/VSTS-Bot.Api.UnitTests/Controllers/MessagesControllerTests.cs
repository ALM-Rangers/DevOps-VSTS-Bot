// ———————————————————————————————
// <copyright file="MessagesControllerTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// $SUMMARY$
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Autofac;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class MessagesControllerTests
    {
        [TestMethod]
        public async Task Post_Activity()
        {
            var activity = new Activity { Type = ActivityTypes.Message, ChannelId = ChannelIds.Directline };

            var telemetryClient = new TelemetryClient();

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => telemetryClient)
                .AsSelf();
            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            var container = builder.Build();
            var botService = new Mock<IBotService>();
            var dialogInvoker = new Mock<IDialogInvoker>();

            var target = new MessagesController(botService.Object, container, dialogInvoker.Object, telemetryClient)
                {
                    Request = new HttpRequestMessage { RequestUri = new Uri("https://somekindofurl/api/messages") }
                };

            var result = await target.Post(activity);

            dialogInvoker.Verify(i => i.SendAsync(activity, It.IsAny<Func<IDialog<object>>>()));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Post_Activity_ConversationUpdate()
        {
            var activity = new Activity { Type = ActivityTypes.ConversationUpdate, ChannelId = ChannelIds.Directline };

            var telemetryClient = new TelemetryClient();

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => telemetryClient)
                .AsSelf();
            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            var container = builder.Build();
            var botService = new Mock<IBotService>();
            var dialogInvoker = new Mock<IDialogInvoker>();

            var target =
                new MessagesController(botService.Object, container, dialogInvoker.Object, telemetryClient)
                {
                    Request = new HttpRequestMessage { RequestUri = new Uri("https://somekindofurl/api/messages") }
                };

            var result = await target.Post(activity);

            dialogInvoker.Verify(i => i.SendAsync(activity, It.IsAny<Func<IDialog<object>>>()));

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [TestMethod]
        public async Task Post_Activity_Exception_Occurs()
        {
            var activity = new Activity { Type = ActivityTypes.Message, ChannelId = ChannelIds.Directline };

            var telemetryClient = new TelemetryClient();

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => telemetryClient)
                .AsSelf();
            builder
                .RegisterType<RootDialog>()
                .AsSelf();

            var container = builder.Build();
            var botService = new Mock<IBotService>();
            var dialogInvoker = new Mock<IDialogInvoker>();

            var target =
                new MessagesController(botService.Object, container, dialogInvoker.Object, telemetryClient)
                {
                    Request = new HttpRequestMessage()
                };

            dialogInvoker.Setup(i => i.SendAsync(activity, It.IsAny<Func<IDialog<object>>>())).Throws<Exception>();

            var result = await target.Post(activity);

            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}