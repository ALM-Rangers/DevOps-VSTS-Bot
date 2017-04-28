// ———————————————————————————————
// <copyright file="AccountConnectDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the AccountConnectionDialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains Test methods for <see cref="AccountConnectDialog"/>
    /// </summary>
    [TestClass]
    public class AccountConnectDialogTests : TestsBase<DialogFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountConnectDialogTests"/> class.
        /// </summary>
        public AccountConnectDialogTests()
            : base(new DialogFixture())
        {
        }

        /// <summary>
        /// Tests the connecting to an account for the first time.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task FirstTimeConnectionTest()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "connect anaccount";

            const string appId = "AnAppId";
            const string authorizeUrl = "https://www.authorizationUrl.com";
            const string scope =
                "vso.agentpools_manage%20vso.build_execute%20vso.chat_manage%20vso.code_manage%20vso.code_status%20" +
                "vso.connected_server%20vso.dashboards_manage%20vso.entitlements%20vso.extension.data_write%20" +
                "vso.extension_manage%20vso.gallery_acquire%20vso.gallery_manage%20vso.identity%20vso.loadtest_write%20" +
                "vso.notification_manage%20vso.packaging_manage%20vso.profile_write%20vso.project_manage%20" +
                "vso.release_manage%20vso.security_manage%20vso.serviceendpoint_manage%20vso.test_write%20vso.work_write";

            var builder = this.Fixture.Build();
            builder.RegisterType<TelemetryClient>();
            builder
                .RegisterType<AccountConnectDialog>()
                .WithParameter("appId", appId)
                .WithParameter("authorizeUrl", new Uri(authorizeUrl))
                .As<IDialog<object>>();

            var container = builder.Build();
            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));
            var root = new RootDialog();

            var toUser = await this.Fixture.GetResponse(container, root, toBot);

            var attachment = toUser.Attachments.FirstOrDefault();
            Assert.IsNotNull(attachment, "Expecting an attachment.");

            var card = attachment.Content as SigninCard;
            Assert.IsNotNull(card, "Missing signin card.");

            var button = card.Buttons.FirstOrDefault();
            Assert.IsNotNull(button, "Button is missing");

            var expected =
                $"https://app.vssps.visualstudio.com/oauth2/authorize?client_id={appId}&response_type=Assertion&state={toBot.ChannelId};{toBot.From.Id}&scope={scope}&redirect_uri={authorizeUrl}/";
            Assert.AreEqual(expected.ToLower(), button.Value.ToString().ToLower(), "OAuth url is invalid.");
        }
    }
}