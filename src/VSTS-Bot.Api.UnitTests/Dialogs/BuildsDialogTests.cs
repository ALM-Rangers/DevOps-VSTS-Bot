// ———————————————————————————————
// <copyright file="BuildsDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the BuildsDialog.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class BuildsDialogTests : TestsBase<DialogFixture>
    {
        public BuildsDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BuildsDialog(this.Fixture.AuthenticationService.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BuildsDialog(null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<BuildsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.BuildsAsync));
        }

        [TestMethod]
        public async Task Builds_Missing_Context()
        {
            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.BuildsAsync(null, null));
        }

        [TestMethod]
        public async Task Builds_Missing_Awaitable()
        {
            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.BuildsAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Builds_No_Text()
        {
            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await target.BuildsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Builds_Empty_List()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "builds";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var buildDefinitions = new List<BuildDefinitionReference>();

            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            this.Fixture.VstsService
                .Setup(s => s.GetBuildDefinitionsAsync(data.Account, data.TeamProject, profile.Token))
                .ReturnsAsync(buildDefinitions);

            await target.BuildsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }

        [TestMethod]
        public async Task Builds()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "builds";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var buildDefinitions = new List<BuildDefinitionReference> { new BuildDefinitionReference { Name = "Build 1" } };

            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            this.Fixture.VstsService
                .Setup(s => s.GetBuildDefinitionsAsync(data.Account, data.TeamProject, profile.Token))
                .ReturnsAsync(buildDefinitions);

            await target.BuildsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();

            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.QueueAsync));
        }

        [TestMethod]
        public async Task Queue_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);
            await target.QueueAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Queue()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "queue 1";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            var target = new BuildsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            this.Fixture.VstsService
                .Setup(s => s.QueueBuildAsync(account, teamProject, 1, profile.Token))
                .ReturnsAsync(new Build { Id = 99 });

            await target.QueueAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService.VerifyAll();
            this.Fixture.DialogContext
                .Verify(
                    dc => dc.PostAsync(
                        It.Is<IMessageActivity>(a => string.Equals(a.Type, ActivityTypes.Typing, StringComparison.Ordinal)),
                        CancellationToken.None),
                    Times.Once);
            this.Fixture.DialogContext
                .Verify(
                    dc => dc.PostAsync(
                        It.Is<IMessageActivity>(a => !string.IsNullOrEmpty(a.Text) && a.Text.Equals("Build with id 99 is queued.", StringComparison.OrdinalIgnoreCase)),
                        CancellationToken.None),
                    Times.Once);

            this.Fixture.DialogContext.Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }
    }
}