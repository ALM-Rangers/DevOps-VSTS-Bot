// ———————————————————————————————
// <copyright file="ConnectDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the AccountConnectionDialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Cards;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Contains Test methods for <see cref="ConnectDialog"/>
    /// </summary>
    [TestClass]
    public class ConnectDialogTests : TestsBase<DialogFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialogTests"/> class.
        /// </summary>
        public ConnectDialogTests()
            : base(new DialogFixture())
        {
        }

        /// <summary>
        /// Tests connecting to an account for the first time.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task Connect_To_An_Account_For_The_First_Time()
        {
            var fromId = Guid.NewGuid().ToString();

            var toBot2 = this.Fixture.CreateMessage();
            toBot2.From.Id = fromId;
            toBot2.Text = "connect anaccount";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";
            const string scope =
                "vso.agentpools%20vso.build_execute%20vso.chat_write%20vso.code%20vso.connected_server%20" +
                "vso.dashboards%20vso.entitlements%20vso.extension%20vso.extension.data%20vso.gallery%20" +
                "vso.identity%20vso.loadtest%20vso.notification%20vso.packaging%20vso.project%20" +
                "vso.release_execute%20vso.serviceendpoint%20vso.taskgroups%20vso.test%20vso.work";

            var wrapper = new Mock<IDialogContextWrapper>();
            var userData = new Mock<IBotDataBag>();

            var builder = this.Fixture.Build();
            builder
                .Register(c => wrapper.Object)
                .As<IDialogContextWrapper>();
            builder.RegisterType<TelemetryClient>();
            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();

            var container = builder.Build();
            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            wrapper
                .Setup(w => w.GetUserData(It.IsAny<IDialogContext>()))
                .Returns(userData.Object);

            var root = new RootDialog(wrapper.Object) { Initialized = true };

            // First trigger the welcome message.
            var toUser = await this.Fixture.GetResponse(container, root, toBot2);

            var attachment = toUser.Attachments.FirstOrDefault();
            Assert.IsNotNull(attachment, "Expecting an attachment.");

            var card = attachment.Content as LogOnCard;
            Assert.IsNotNull(card, "Missing signin card.");

            var button = card.Buttons.FirstOrDefault();
            Assert.IsNotNull(button, "Button is missing");

            var expected =
                FormattableString.Invariant($"https://app.vssps.visualstudio.com/oauth2/authorize?client_id={appId}&response_type=Assertion&state={toBot2.ChannelId};{toBot2.From.Id}&scope={scope}&redirect_uri={authorizeUrl}/");
            Assert.AreEqual(expected.ToLower(), button.Value.ToString().ToLower(), "OAuth url is invalid.");
        }

        /// <summary>
        /// Tests connecting to an account for the second time.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task Connect_To_An_Account_Where_Previously_Connected_To()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.From.Name = "User";
            toBot.Text = "connect anaccount ateamproject";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";

            var account = "anaccount";
            var profile = new VstsProfile();
            IList<VstsProfile> profiles = new List<VstsProfile> { profile };
            var teamProject = "TeamProject1";

            var wrapper = new Mock<IDialogContextWrapper>();
            var userData = new Mock<IBotDataBag>();

            var builder = this.Fixture.Build();
            builder
                .Register(c => wrapper.Object)
                .As<IDialogContextWrapper>();
            builder.RegisterType<TelemetryClient>();
            builder
                .RegisterType<ConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();
            var container = builder.Build();
            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            wrapper
                .Setup(w => w.GetUserData(It.IsAny<IDialogContext>()))
                .Returns(userData.Object);
            userData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            userData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            userData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
                .Returns(true);
            userData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var root = new RootDialog(wrapper.Object) { Initialized = true };

            var toUser = await this.Fixture.GetResponse(container, root, toBot);

            Assert.AreEqual("Connected to anaccount / ateamproject.", toUser.Text);
        }
    }
}