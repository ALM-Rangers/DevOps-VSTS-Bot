// ———————————————————————————————
// <copyright file="ConnectDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the ConnectDialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using TSBot.Cards;

    /// <summary>
    /// Tests for <see cref="ConnectDialog"/>.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class ConnectDialogTests : TestsBase<DialogFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialogTests"/> class.
        /// </summary>
        public ConnectDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_AppId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog(null, "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AppScope()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog("appId", null, new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthorizeUrl()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog("appId", "appScope", null, this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ConnectAsync));
        }

        [TestMethod]
        public async Task Connect_Missing_Context()
        {
            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ConnectAsync(null, null));
        }

        [TestMethod]
        public async Task Connect_Missing_Awaitable()
        {
            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ConnectAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Connect_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            UserData data = null;

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(false);

            mocked
                .Setup(m => m.LogOnAsync(this.Fixture.DialogContext.Object, toBot))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Connect_For_The_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect";

            UserData data = null;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(false);

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            mocked
                .Setup(m => m.LogOnAsync(this.Fixture.DialogContext.Object, toBot))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Connect_For_The_Second_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect";

            var data = new UserData();
            data.Profiles.Add(this.Fixture.CreateProfile());

            var builder = new ContainerBuilder();
            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            mocked
                .Setup(m => m.SelectAccountAsync(this.Fixture.DialogContext.Object, toBot))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Connect_For_The_Second_Time_With_Account_Selected()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account";

            var data = new UserData();
            data.Profiles.Add(this.Fixture.CreateProfile());

            var builder = new ContainerBuilder();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            mocked
                .Setup(m => m.SelectProjectAsync(this.Fixture.DialogContext.Object, toBot))
                .Returns(Task.CompletedTask)
                .Verifiable();

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Connect_For_The_Second_Time_With_Account_And_TeamProject_Selected()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account teamproject";

            var data = new UserData();
            data.Profiles.Add(this.Fixture.CreateProfile());

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals($"Dear {toBot.From.Name}. I have connected you to your team project 'teamproject', which is in account 'account'.", StringComparison.OrdinalIgnoreCase)), CancellationToken.None));
            this.Fixture.DialogContext
                .Verify(c => c.Done(It.IsAny<IMessageActivity>()));

            mocked.Verify();
        }

        [TestMethod]
        public async Task LogOn()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account teamproject";

            var data = new UserData();

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            await target.LogOnAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.UserData.Verify(ud => ud.SetValue("userData", data));
            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.Is<IMessageActivity>(a => a.Attachments.First().Content is LogOnCard), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.PinReceivedAsync));

            data.Pin.Should().MatchRegex(@"\d{5}");
        }

        [TestMethod]
        public async Task LogOn_NoUserData()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account teamproject";

            UserData data;

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(false);

            await target.LogOnAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.UserData.Verify(ud => ud.SetValue("userData", It.IsAny<UserData>()));
            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.Is<IMessageActivity>(a => a.Attachments.First().Content is LogOnCard), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.PinReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Received_Pin()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "12345";

            var data = new UserData();
            data.Profiles.Add(this.Fixture.CreateProfile());
            data.Profile.IsValidated = false;

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Pin = "12345";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.PinReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            target.Profile.Should().Be(data.Profile);
            data.Profiles.Should().Contain(data.Profile);
            this.Fixture.UserData.Verify(ud => ud.SetValue("userData", data));
            mocked.Verify();
        }

        [TestMethod]
        public async Task Handle_Received_Pin_Which_Is_Invalid()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "00000";

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { Pin = "12345" };

            await target.PinReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals("Sorry, I do not recognize the provided pin. Please try again.", StringComparison.OrdinalIgnoreCase)),
                    CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.PinReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Received_No_Pin()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { Pin = "12345" };

            await target.PinReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals("Sorry, I do not recognize the provided pin. Please try again.", StringComparison.OrdinalIgnoreCase)),
                    CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.PinReceivedAsync));
        }

        [TestMethod]
        public async Task Select_Account()
        {
            var profile1 = new Profile { Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new Profile { Accounts = new List<string> { "Account3", "Account4" } };
            var profiles = new List<Profile> { profile1, profile2 };

            var toBot = this.Fixture.CreateMessage();

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { Profiles = profiles };

            await target.SelectAccountAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.AccountReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Unknown_Account_Received()
        {
            var profile1 = new Profile { Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new Profile { Accounts = new List<string> { "Account3", "Account4" } };
            var profiles = new List<Profile> { profile1, profile2 };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "UnknownAccount";

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Profiles = profiles;

            mocked.Setup(m => m.LogOnAsync(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.AccountReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Handle_Account_Received()
        {
            var profile1 = new Profile { Id = Guid.NewGuid(), Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new Profile { Id = Guid.NewGuid(), Accounts = new List<string> { "Account3", "Account4" }, IsSelected = true, IsValidated = true };

            var data = new UserData { Profiles = new List<Profile> { profile1, profile2 } };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Account3";

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Profiles = data.Profiles;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.AccountReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.UserData.Verify(ud => ud.SetValue("userData", data));
            mocked.Verify();

            data.Account.Should().Be("Account3");
            data.Profile.Should().Be(profile2);
        }

        [TestMethod]
        public async Task Select_Project()
        {
            var account = "Account1";
            var profile = new Profile { Accounts = new List<string> { account }, Token = new OAuthToken() };
            var projects = new List<TeamProjectReference> { new TeamProjectReference { Name = "Project1" } };

            var toBot = this.Fixture.CreateMessage();

            var target = new ConnectDialog("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { Account = account, Profile = profile };

            this.Fixture.VstsService.Setup(s => s.GetProjects(account, profile.Token)).ReturnsAsync(projects).Verifiable();

            await target.SelectProjectAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.VstsService.Verify();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ProjectReceivedAsync));

            target.TeamProjects.Should().Contain("Project1");
        }

        [TestMethod]
        public async Task Handle_Project_Received()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Project1";

            var data = new UserData();

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.TeamProjects = new List<string> { "Project1" };

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.ProjectReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.UserData.Verify(ud => ud.SetValue("userData", data));
            mocked.Verify();

            data.TeamProject.Should().Be("Project1");
        }

        [TestMethod]
        public async Task Handle_Project_Received_Invalid_TeamProject()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Project2";

            var mocked = new Mock<ConnectDialog>("appId", "appScope", new Uri("http://authorize.url"), this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.TeamProjects = new List<string> { "Project1" };

            mocked.Setup(m => m.SelectProjectAsync(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.ProjectReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }
    }
}