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
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.WebApi;
    using Cards;
    using Common.Tests;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
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
        public void Constructor_Missing_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RootDialog(null, null));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor_Missing_TelemetryClient()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RootDialog(this.Fixture.AuthenticationService.Object, null));
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "target", Justification = "Test for constructor only.")]
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor()
        {
            var target = new RootDialog(this.Fixture.AuthenticationService.Object, new TelemetryClient());
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<RootDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.HandleActivityAsync));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Welcome_No_Message()
        {
            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, null);
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
        public async Task Welcome_Second_Time_No_Account_And_No_TeamProject()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Type = ActivityTypes.ConversationUpdate;
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testUser", Name = "testUser" });
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testBot", Name = "testBot" });

            var profile = this.Fixture.CreateProfile();

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);

            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Text.Equals($"Welcome back {toBot.From.Name}. I have connected you to Account '?', Team Project '?'.", StringComparison.Ordinal)),
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

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Forward_Command()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "connect";

            var vstsService = new Mock<IVstsService>();
            var dialog = new ConnectDialog("appid", new Uri("https://someurl.com"), vstsService.Object);

            var container = new ContainerBuilder();
            container
                .RegisterModule<AttributedMetadataModule>();
            container
                .Register((c, x) => dialog)
                .As<IDialog<object>>();
            var build = container.Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(build));

            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.HandleCommandAsync(this.Fixture.DialogContext.Object, toBot);

            // TODO:
            this.Fixture.DialogContext
                .Verify(c => c.Forward<object, IMessageActivity>(dialog, target.ResumeAfterChildDialog, toBot, CancellationToken.None));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Resume_After_Child_Finishes()
        {
            var target = new RootDialog(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient);

            await target.ResumeAfterChildDialog(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(string.Empty));

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.HandleActivityAsync));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Handle_Message_Activity()
        {
            var message = this.Fixture.CreateMessage();

            var mocked = new Mock<RootDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient) { CallBase = true };
            var target = mocked.Object;

            mocked.Setup(m => m.HandleCommandAsync(this.Fixture.DialogContext.Object, message)).Returns(Task.CompletedTask).Verifiable();

            await target.HandleActivityAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(message));

            mocked.Verify();
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Handle_Update_Activity()
        {
            var message = this.Fixture.CreateMessage();
            message.Type = ActivityTypes.ConversationUpdate;

            var mocked = new Mock<RootDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.TelemetryClient) { CallBase = true };
            var target = mocked.Object;

            mocked.Setup(m => m.WelcomeAsync(this.Fixture.DialogContext.Object, message)).Returns(Task.CompletedTask).Verifiable();

            await target.HandleActivityAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(message));

            mocked.Verify();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.Fixture.Dispose();
        }
    }
}