// ———————————————————————————————
// <copyright file="EventControllerTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for the Event Controller.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Events;
    using FluentAssertions;
    using Microsoft.Azure.Documents;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Strategies.Event;

    [TestClass]
    [TestCategory("Unit")]
    public class EventControllerTests
    {
        [TestMethod]
        public void Constructor_Missing_DocumentClient()
        {
            var strategies = new List<IEventStrategy>();

            Assert.ThrowsException<ArgumentNullException>(() => new EventController(null, strategies));
        }

        [TestMethod]
        public void Constructor_Missing_Strategies()
        {
            var dcMock = new Mock<IDocumentClient>();

            Assert.ThrowsException<ArgumentNullException>(() => new EventController(dcMock.Object, null));
        }

        [TestMethod]
        public async Task Post_Missing_SubscriptionId_In_Header()
        {
            var @event = new Event<ApprovalResource>();
            var strategies = new List<IEventStrategy>();

            var dcMock = new Mock<IDocumentClient>();

            var target = new EventController(dcMock.Object, strategies)
            {
                Request = new HttpRequestMessage()
            };
            target.Request.SetConfiguration(new HttpConfiguration());

            var result = await target.Post(@event);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Post_Invalid_SubscriptionId_In_Header()
        {
            var @event = new Event<ApprovalResource>();
            var strategies = new List<IEventStrategy>();

            var dcMock = new Mock<IDocumentClient>();

            var target = new EventController(dcMock.Object, strategies)
            {
                Request = new HttpRequestMessage { Headers = { { "subscriptionToken", "Invalid" } } }
            };
            target.Request.SetConfiguration(new HttpConfiguration());

            var result = await target.Post(@event);

            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public async Task Post()
        {
            var @event = new Event<ApprovalResource>();
            var subscription = new Subscription();
            var strategies = new List<IEventStrategy>();

            var dcMock = new Mock<IDocumentClient>();
            var strategyMock = new Mock<IEventStrategy>();
            strategies.Add(strategyMock.Object);

            var target = new EventController(dcMock.Object, strategies)
            {
                Request = new HttpRequestMessage { Headers = { { "subscriptionToken", subscription.Id.ToString() } } }
            };
            target.Request.SetConfiguration(new HttpConfiguration());

            dcMock
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), It.IsAny<SqlQuerySpec>(), null))
                .Returns(new List<Subscription> { subscription }.AsQueryable().OrderBy(s => s.Id));
            strategyMock
                .Setup(s => s.ShouldHandle(@event))
                .Returns(true);
            strategyMock
                .Setup(s => s.Handle(@event, subscription))
                .Returns(Task.CompletedTask);

            var result = await target.Post(@event);

            result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}