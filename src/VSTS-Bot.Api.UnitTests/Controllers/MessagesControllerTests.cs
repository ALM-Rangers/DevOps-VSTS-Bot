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
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class MessagesControllerTests
    {
        [TestMethod]
        public async Task Post_Activity()
        {
            using (ShimsContext.Create())
            {
                var isValidDialog = false;

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

                var target = new MessagesController(container, telemetryClient)
                {
                    Request = new HttpRequestMessage { RequestUri = new Uri("https://somekindofurl/api/messages") }
                };

                Microsoft.Bot.Builder.Dialogs.Fakes.ShimConversation.SendAsyncIMessageActivityFuncOfIDialogOfObjectCancellationToken =
                    (a, d, t) =>
                    {
                        isValidDialog = d() is RootDialog;

                        return Task.CompletedTask;
                    };

                var result = await target.Post(activity);

                isValidDialog.Should().BeTrue();
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [TestMethod]
        public async Task Post_Activity_ConversationUpdate()
        {
            using (ShimsContext.Create())
            {
                var isValidDialog = false;

                var activity = new Activity
                {
                    Type = ActivityTypes.ConversationUpdate,
                    ChannelId = ChannelIds.Directline
                };

                var telemetryClient = new TelemetryClient();

                var builder = new ContainerBuilder();
                builder
                    .Register((c, x) => telemetryClient)
                    .AsSelf();
                builder
                    .RegisterType<RootDialog>()
                    .AsSelf();

                var container = builder.Build();

                var target =
                    new MessagesController(container, telemetryClient)
                    {
                        Request = new HttpRequestMessage { RequestUri = new Uri("https://somekindofurl/api/messages") }
                    };

                Microsoft.Bot.Builder.Dialogs.Fakes.ShimConversation.SendAsyncIMessageActivityFuncOfIDialogOfObjectCancellationToken =
                    (a, d, t) =>
                    {
                        isValidDialog = d() is RootDialog;

                        return Task.CompletedTask;
                    };

                var result = await target.Post(activity);

                isValidDialog.Should().BeTrue();
                result.StatusCode.Should().Be(HttpStatusCode.OK);
            }
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

            var target =
                new MessagesController(container, telemetryClient)
                {
                    Request = new HttpRequestMessage()
                };

            var result = await target.Post(activity);

            result.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}