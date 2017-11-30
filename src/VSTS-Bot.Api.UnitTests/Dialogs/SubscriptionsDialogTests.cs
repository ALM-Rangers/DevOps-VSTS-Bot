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
    using System.Threading;
    using System.Threading.Tasks;
    using Dialogs;
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
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<SubscriptionsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.SubscriptionsAsync));
        }

        [TestMethod]
        public async Task Builds_Missing_Context()
        {
            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(null, null));
        }

        [TestMethod]
        public async Task Builds_Missing_Awaitable()
        {
            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Subscriptions_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

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

            var target = new SubscriptionsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            await target.SubscriptionsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.SubscribeAsync));
        }
    }
}