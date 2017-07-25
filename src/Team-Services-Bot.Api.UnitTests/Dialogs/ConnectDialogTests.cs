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
    using Cards;
    using Common.Tests;
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
    [TestCategory(TestCategories.Unit)]
    [ExcludeFromCodeCoverage]
    public class ConnectDialogTests : TestsBase<DialogFixture>
    {
        private const string AppId = "AnAppId";
        private const string AppScope = "AnAppScope";
        private const string AuthorizeUrl = "https://www.authorizationUrl.com";

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
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog(null, null, null, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AppScope()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog(AppId, null, null, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_AuthorizeUrl()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog(AppId, AppScope, null, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<ConnectDialog>(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ConnectAsync));
        }

        [TestMethod]
        public async Task Connect_Missing_Context()
        {
            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ConnectAsync(null, null));
        }

        [TestMethod]
        public async Task Connect_Missing_Awaitable()
        {
            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.ConnectAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Connect_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

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

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
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

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var profile = new VstsProfile { Token = new OAuthToken { ExpiresIn = 3600 } };
            var profiles = new List<VstsProfile> { profile } as IList<VstsProfile>;

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => this.Fixture.AuthenticationService.Object)
                .As<IAuthenticationService>();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
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
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account";

            var profile = new VstsProfile { Token = new OAuthToken { ExpiresIn = 3600 } };
            var profiles = new List<VstsProfile> { profile } as IList<VstsProfile>;

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => this.Fixture.AuthenticationService.Object)
                .As<IAuthenticationService>();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
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
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account teamproject";

            var profile = new VstsProfile { Token = new OAuthToken { ExpiresIn = 3600 } };
            IList<VstsProfile> profiles = new List<VstsProfile> { profile };

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
                .Returns(true);

            await target.ConnectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals("Connected to account / teamproject.", StringComparison.OrdinalIgnoreCase)), CancellationToken.None));
            this.Fixture.DialogContext
                .Verify(c => c.Done(It.IsAny<IMessageActivity>()));

            mocked.Verify();
        }

        [TestMethod]
        public async Task LogOn()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect account teamproject";

            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object);

            await target.LogOnAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.UserData.Verify(ud => ud.SetValue("Pin", It.IsRegex(@"\d{5}")));
            this.Fixture.DialogContext.Verify(c => c.PostAsync(It.Is<IMessageActivity>(a => a.Attachments.First().Content is LogOnCard), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.PinReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Received_Pin()
        {
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "12345";

            var profile = new VstsProfile();
            var profiles = new List<VstsProfile>() as IList<VstsProfile>;

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Pin = "12345";

            this.Fixture.UserData.Setup(ud => ud.TryGetValue("NotValidatedByPinProfile", out profile)).Returns(true);
            this.Fixture.UserData.Setup(ud => ud.TryGetValue("Profiles", out profiles)).Returns(true);

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.PinReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            target.Profile.Should().Be(profile);
            profiles.Should().Contain(profile);
            this.Fixture.UserData.Verify(ud => ud.SetValue("Profile", profile));
            mocked.Verify();
        }

        [TestMethod]
        public async Task Handle_Received_Pin_Which_Is_Invalid()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "00000";

            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object) { Pin = "12345" };

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

            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object) { Pin = "12345" };

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
            var profile1 = new VstsProfile { Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new VstsProfile { Accounts = new List<string> { "Account3", "Account4" } };
            var profiles = new List<VstsProfile> { profile1, profile2 };

            var toBot = this.Fixture.CreateMessage();

            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object) { Profiles = profiles };

            await target.SelectAccountAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Attachments.First().Content is AccountsCard),
                    CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.AccountReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Unknown_Account_Received()
        {
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var profile1 = new VstsProfile { Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new VstsProfile { Accounts = new List<string> { "Account3", "Account4" } };
            var profiles = new List<VstsProfile> { profile1, profile2 };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "UnknownAccount";

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Profiles = profiles;

            mocked.Setup(m => m.LogOnAsync(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.AccountReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Handle_Account_Received()
        {
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var profile1 = new VstsProfile { Accounts = new List<string> { "Account1", "Account2" } };
            var profile2 = new VstsProfile { Accounts = new List<string> { "Account3", "Account4" } };
            var profiles = new List<VstsProfile> { profile1, profile2 };

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Account3";

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.Profiles = profiles;

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.AccountReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.UserData.Verify(ud => ud.SetValue("Account", "Account3"));
            this.Fixture.UserData.Verify(ud => ud.SetValue("Profile", profile2));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Select_Project()
        {
            var account = "Account1";
            var profile = new VstsProfile { Accounts = new List<string> { account }, Token = new OAuthToken() };
            var projects = new List<TeamProjectReference> { new TeamProjectReference() };

            var toBot = this.Fixture.CreateMessage();

            var target = new ConnectDialog(AppId, AppScope, new Uri(AuthorizeUrl), this.Fixture.VstsService.Object) { Account = account, Profile = profile };

            this.Fixture.VstsService.Setup(s => s.GetProjects(account, profile.Token)).ReturnsAsync(projects).Verifiable();

            await target.SelectProjectAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.VstsService.Verify();
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Attachments.First().Content is ProjectsCard),
                    CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ProjectReceivedAsync));
        }

        [TestMethod]
        public async Task Handle_Project_Received()
        {
            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Project1";

            var mocked = new Mock<ConnectDialog>(appId, new Uri(authorizeUrl), this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            mocked.Setup(m => m.ContinueProcess(this.Fixture.DialogContext.Object, toBot)).Returns(Task.CompletedTask).Verifiable();

            await target.ProjectReceivedAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.UserData.Verify(ud => ud.SetValue("TeamProject", "Project1"));
            mocked.Verify();
        }
    }
}