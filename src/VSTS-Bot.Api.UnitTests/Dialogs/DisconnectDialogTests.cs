// ———————————————————————————————
// <copyright file="DisconnectDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the DisconnectDialog.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    public class DisconnectDialogTests : TestsBase<DialogFixture>
    {
        public DisconnectDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DisconnectDialog(null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new DisconnectDialog(this.Fixture.AuthenticationService.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<DisconnectDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.DisconnectAsync));
        }

        [TestMethod]
        public async Task Disconnect_Missing_Context()
        {
            var target = new DisconnectDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.DisconnectAsync(null, null));
        }

        [TestMethod]
        public async Task Disconnect_Missing_Awaitable()
        {
            var target = new DisconnectDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.DisconnectAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Disconnect_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject", };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            var target = new DisconnectDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);
            await target.DisconnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Disconnect_SetValueFailed_False()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "disconnect";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject",  };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            this.Fixture.UserData
                .Setup(ud => ud.RemoveValue("userData"))
                .Returns(false);

            var target = new DisconnectDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);
            await target.DisconnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Disconnect_SetValueFailed_True()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "disconnect";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject", };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            this.Fixture.UserData
                .Setup(ud => ud.RemoveValue("userData"))
                .Returns(true);

            var target = new DisconnectDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);
            await target.DisconnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }
    }
}