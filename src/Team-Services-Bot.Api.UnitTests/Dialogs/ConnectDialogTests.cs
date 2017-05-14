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
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Cards;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ConnectDialogTests : TestsBase<DialogFixture>
    {
        public ConnectDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Connect_To_An_Account_For_The_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect anaccount";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var builder = new ContainerBuilder();
            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();

            var container = this.Fixture.Build(builder);

            this.Fixture.RootDialog.Initialized = true;

            // First trigger the welcome message.
            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            var attachment = toUser.Attachments.FirstOrDefault();
            attachment.Should().NotBeNull();

            var card = attachment.Content;
            card.Should().BeOfType<LogOnCard>();

            this.Fixture.UserData.Verify(ud => ud.SetValue("Pin", It.IsRegex("\\d{4}")));
        }

        [TestMethod]
        public async Task Connect_To_An_Account_Select_An_Account()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var profile = new VstsProfile();
            IList<VstsProfile> profiles = new List<VstsProfile> { profile };

            var builder = new ContainerBuilder();
            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();

            var container = this.Fixture.Build(builder);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
                .Returns(true);

            this.Fixture.RootDialog.Initialized = true;

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            var attachment = toUser.Attachments.FirstOrDefault();
            attachment.Should().NotBeNull();

            var card = attachment.Content;
            card.Should().BeOfType<AccountsCard>();
        }

        [TestMethod]
        public async Task Connect_To_An_Account_Where_Previously_Connected_To()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect anaccount ateamproject";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var account = "anaccount";
            var profile = new VstsProfile();
            IList<VstsProfile> profiles = new List<VstsProfile> { profile };
            var teamProject = "TeamProject1";

            var builder = new ContainerBuilder();
            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();

            var container = this.Fixture.Build(builder);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            this.Fixture.RootDialog.Initialized = true;

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            toUser.Text.Should().Be("Connected to anaccount / ateamproject.");
        }
    }
}