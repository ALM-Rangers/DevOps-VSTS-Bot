// ———————————————————————————————
// <copyright file="ReleasesDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the ReleasesDialog.
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
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    public class ReleasesDialogTests : TestsBase<DialogFixture>
    {
        public ReleasesDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ReleasesDialog(null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<ReleasesDialog>(this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ReleasesAsync));
        }

        [TestMethod]
        public async Task Releases_Missing_Context()
        {
            var target = new ReleasesDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ReleasesAsync(null, null));
        }

        [TestMethod]
        public async Task Releases_Missing_Awaitable()
        {
            var target = new ReleasesDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ReleasesAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Releases_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new ReleasesDialog(this.Fixture.VstsService.Object);
            await target.ReleasesAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Releases_Empty_List()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "releases";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            var releaseDefinitions = new List<ReleaseDefinition>();

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            this.Fixture.VstsService
                .Setup(s => s.GetReleaseDefinitionsAsync(account, teamProject, profile.Token))
                .ReturnsAsync(() => releaseDefinitions);

            var target = new ReleasesDialog(this.Fixture.VstsService.Object);
            await target.ReleasesAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));

            this.Fixture.DialogContext.Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }

        [TestMethod]
        public async Task Releases()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "releases";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            var releaseDefinitions = new List<ReleaseDefinition> { new ReleaseDefinition { Id = 1 } };

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            this.Fixture.VstsService
                .Setup(s => s.GetReleaseDefinitionsAsync(account, teamProject, profile.Token))
                .ReturnsAsync(() => releaseDefinitions);

            var target = new ReleasesDialog(this.Fixture.VstsService.Object);
            await target.ReleasesAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.CreateAsync));
        }

        [TestMethod]
        public async Task Create_Missing_Context()
        {
            var target = new ReleasesDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.CreateAsync(null, null));
        }

        [TestMethod]
        public async Task Create_Missing_Awaitable()
        {
            var target = new ReleasesDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.CreateAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Create_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new ReleasesDialog(this.Fixture.VstsService.Object);
            await target.CreateAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Create()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "create 1";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            var release = new Release();

            var target = new ReleasesDialog(this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            this.Fixture.VstsService
                .Setup(s => s.CreateReleaseAsync(account, teamProject, 1, profile.Token))
                .ReturnsAsync(release);

            await target.CreateAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }
    }
}