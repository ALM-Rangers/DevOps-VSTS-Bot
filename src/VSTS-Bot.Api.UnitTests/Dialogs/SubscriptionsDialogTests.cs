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
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using Strategies.Subscription;

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
            var strategies = new List<ISubscriptionStrategy>();

            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(null, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_DocumentClient()
        {
            var strategies = new List<ISubscriptionStrategy>();

            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, null, strategies, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_Strategies()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            var strategies = new List<ISubscriptionStrategy>();

            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<SubscriptionsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.SubscriptionsAsync));
        }

        [TestMethod]
        public async Task Builds_Missing_Context()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(null, null));
        }

        [TestMethod]
        public async Task Builds_Missing_Awaitable()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Subscriptions_No_Text()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Subscriptions()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "subscriptions";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var subscription = new Subscription();
            var subscriptions = new List<Subscription> { subscription };

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

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
            var strategies = new List<ISubscriptionStrategy>();

            var toBot = this.Fixture.CreateMessage();

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.SubscribeAsync(null, this.Fixture.MakeAwaitable(toBot)));
        }

        [TestMethod]
        public async Task Subscribe_Missing_Result()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => target.SubscribeAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Subscribe_Empty_Text()
        {
            var strategies = new List<ISubscriptionStrategy>();

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = string.Empty;

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object);

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Subscribe_MyApprovals()
        {
            var strategies = new List<ISubscriptionStrategy> { new MyApprovalSubscriptionStrategy() };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "subscribe MyApprovals";

            var account = "anAccount";
            var profile = new Profile { Token = new OAuthToken() };
            var teamProject = "TeamProject1";
            var teamProjects = new List<TeamProjectReference> { new TeamProjectReference { Name = teamProject } };

            var subscription = new VSTS_Bot.TeamFoundation.Services.WebApi.Subscription { CreatedBy = new IdentityRef { Id = "1" } };
            var subscriptions = new List<Subscription>();

            this.Fixture.DocumentClient
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), It.IsAny<SqlQuerySpec>(), null))
                .Returns(subscriptions.AsQueryable().OrderBy(s => s.Id));
            this.Fixture.VstsService
                .Setup(s => s.GetProjects(account, profile.Token))
                .ReturnsAsync(teamProjects);
            this.Fixture.VstsService
                .Setup(s => s.CreateSubscription(account, It.IsAny<VSTS_Bot.TeamFoundation.Services.WebApi.Subscription>(), profile.Token))
                .ReturnsAsync(subscription);
            this.Fixture.DocumentClient
                .Setup(dc => dc.UpsertDocumentAsync(It.IsAny<Uri>(), It.IsAny<Subscription>(), null, false))
                .Returns(Task.FromResult(new ResourceResponse<Document>()));

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DocumentClient.VerifyAll();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(dc => dc.Done(It.IsAny<IMessageActivity>()));
        }

        [TestMethod]
        public async Task Unsubscribe_MyApprovals()
        {
            var strategies = new List<ISubscriptionStrategy> { new MyApprovalSubscriptionStrategy() };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "unsubscribe MyApprovals";

            var account = "anAccount";
            var profile = new Profile { Token = new OAuthToken() };
            var teamProject = "TeamProject1";

            var subscription = new Subscription
            {
                ChannelId = toBot.ChannelId,
                SubscriptionId = Guid.NewGuid(),
                SubscriptionType = SubscriptionType.MyApprovals,
                UserId = toBot.From.Id
            };
            var subscriptions = new List<Subscription> { subscription };

            this.Fixture.DocumentClient
                .Setup(dc => dc.CreateDocumentQuery<Subscription>(It.IsAny<Uri>(), It.IsAny<SqlQuerySpec>(), null))
                .Returns(subscriptions.AsQueryable().OrderBy(s => s.Id));
            this.Fixture.DocumentClient
                .Setup(dc => dc.DeleteDocumentAsync(It.IsAny<Uri>(), null))
                .ReturnsAsync(new ResourceResponse<Document>());
            this.Fixture.VstsService
                .Setup(s => s.DeleteSubscription(account, subscription.SubscriptionId, profile.Token))
                .Returns(Task.CompletedTask);

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.DocumentClient.Object, strategies, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.SubscribeAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DocumentClient.VerifyAll();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(dc => dc.Done(It.IsAny<IMessageActivity>()));
        }
    }
}