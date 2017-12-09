// ———————————————————————————————
// <copyright file="SubscriptionsDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the SubscriptionsDialog.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Dialogs;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    public class SubscriptionsDialogTests : TestsBase<DialogFixture>
    {
        public SubscriptionsDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(null, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_DocumentClient()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<SubscriptionsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.SubscriptionsAsync));
        }

        [TestMethod]
        public async Task Builds_Missing_Context()
        {
            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(null, null));
        }

        [TestMethod]
        public async Task Builds_Missing_Awaitable()
        {
            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Subscriptions_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Subscriptions()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "subscriptions";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var subscription = new Subscription();
            var subscriptions = new List<Subscription> { subscription };

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);
            this.Fixture.DocumentClient
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), null))
                .Returns(subscriptions.AsQueryable().OrderBy(s => s.Id));

            await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.SubscribeAsync));
        }

        [TestMethod]
        public async Task Subscribe_Missing_Context()
        {
            var toBot = this.Fixture.CreateMessage();

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.SubscribeAsync(null, this.Fixture.MakeAwaitable(toBot)));
        }

        [TestMethod]
        public async Task Subscribe_Missing_Result()
        {
            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.SubscribeAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Subscribe_Empty_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = string.Empty;

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Subscribe_MyApprovals()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "subscribe MyApprovals";

            var subscriptions = new List<Subscription>();

            this.Fixture.DocumentClient
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), null))
                .Returns(subscriptions.AsQueryable().OrderBy(s => s.Id));
            this.Fixture.DocumentClient
                .Setup(dc => dc.UpsertDocumentAsync(It.IsAny<Uri>(), It.IsAny<Subscription>(), null, false))
                .Returns(Task.FromResult(new ResourceResponse<Document>()));

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DocumentClient.VerifyAll();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(dc => dc.Done(It.IsAny<IMessageActivity>()));
        }

        [TestMethod]
        public async Task Unsubscribe_MyApprovals()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "unsubscribe MyApprovals";

            var subscription = new Subscription
            {
                ChannelId = toBot.ChannelId,
                SubscriptionType = SubscriptionType.MyApprovals,
                UserId = toBot.From.Id
            };
            var subscriptions = new List<Subscription> { subscription };

            this.Fixture.DocumentClient
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), null))
                .Returns(subscriptions.AsQueryable().OrderBy(s => s.Id));
            this.Fixture.DocumentClient
                .Setup(dc => dc.DeleteDocumentAsync(It.IsAny<Uri>(), null))
                .Returns(Task.FromResult(new ResourceResponse<Document>()));

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, this.Fixture.VstsService.Object);

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DocumentClient.VerifyAll();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(dc => dc.Done(It.IsAny<IMessageActivity>()));
        }
    }
}