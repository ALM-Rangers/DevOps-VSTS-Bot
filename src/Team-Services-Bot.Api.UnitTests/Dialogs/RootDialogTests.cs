// ———————————————————————————————
// <copyright file="RootDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the RootDialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
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
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose is called during cleanup.")]
        public RootDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Welcome_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Type = ActivityTypes.ConversationUpdate;
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testUser", Name = "testUser" });
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testBot", Name = "testBot" });

            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals($"Welcome {toBot.From.Name}. This is the first time we talk.", StringComparison.Ordinal)),
                    CancellationToken.None));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Welcome_Second_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Type = ActivityTypes.ConversationUpdate;
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testUser", Name = "testUser" });
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testBot", Name = "testBot" });

            var account = "anaccount";

            var profile = this.Fixture.CreateProfile();
            var teamProject = "TeamProject1";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals($"Welcome back {toBot.From.Name}. I have connected you to Account '{account}', Team Project '{teamProject}'.", StringComparison.Ordinal)),
                    CancellationToken.None));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Show_Options()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var container = new ContainerBuilder().Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.HandleCommandAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Attachments.First().Content is MainOptionsCard),
                    CancellationToken.None));
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Fixture.Dispose();
        }
    }
}