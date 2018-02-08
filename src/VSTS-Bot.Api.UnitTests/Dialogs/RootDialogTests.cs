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
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Autofac.Extras.AttributeMetadata;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Internals.Fibers;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using TSBot.Cards;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Dispose is called during cleanup.")]
        public RootDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public void Constructor_Missing_LicenseUri()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RootDialog(null, null));
        }

        [TestMethod]
        public void Constructor_Missing_TelemetryClient()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RootDialog(new Uri("https://an.url.toLicense"), null));
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "target", Justification = "Test for constructor only.")]
        [TestMethod]
        public void Constructor()
        {
            var target = new RootDialog(new Uri("https://an.url.toLicense"), new TelemetryClient());
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<RootDialog>(new Uri("https://an.url.toLicense"), this.Fixture.TelemetryClient) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.HandleActivityAsync));
        }

        [TestMethod]
        public async Task Welcome_No_Message()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var target = this.Fixture.RootDialog;

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, toBot);
        }

        [TestMethod]
        public async Task Welcome_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Type = ActivityTypes.ConversationUpdate;
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testUser", Name = "testUser" });
            toBot.MembersAdded.Add(new ChannelAccount { Id = "testBot", Name = "testBot" });

            var target = this.Fixture.RootDialog;

            await target.WelcomeAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => Regex.IsMatch(a.Text, $"Hi {toBot.From.Name}. Good to see you. I will help you with your Visual Studio Team Services tasks. Please read the \\[MIT License\\]\\(.+\\) if you have not done so.")),
                    CancellationToken.None));
        }

        [TestMethod]
        public async Task Show_Options()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            var container = new ContainerBuilder().Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            var target = this.Fixture.RootDialog;

            await target.HandleCommandAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(
                    It.Is<IMessageActivity>(a => a.Attachments.First().Content is MainOptionsCard),
                    CancellationToken.None));
        }

        [TestMethod]
        public async Task Forward_Command()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "builds";

            var dialog = new BuildsDialog(new Mock<IAuthenticationService>().Object, new Mock<IVstsService>().Object);

            var container = new ContainerBuilder();
            container
                .RegisterModule<AttributedMetadataModule>();
            container
                .Register((c, x) => dialog)
                .As<IDialog<object>>();
            var build = container.Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(build));

            var target = this.Fixture.RootDialog;

            await target.HandleCommandAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.Forward<object, IMessageActivity>(dialog, target.ResumeAfterChildDialog, toBot, CancellationToken.None));
        }

        [TestMethod]
        public async Task InGroup_Command()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "builds";
            toBot.Conversation.IsGroup = true;

            var dialog = new BuildsDialog(new Mock<IAuthenticationService>().Object, new Mock<IVstsService>().Object);

            var container = new ContainerBuilder();
            container
                .RegisterModule<AttributedMetadataModule>();
            container
                .Register((c, x) => dialog)
                .As<IDialog<object>>();
            var build = container.Build();

            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(build));

            var target = this.Fixture.RootDialog;

            await target.HandleCommandAsync(this.Fixture.DialogContext.Object, toBot);

            this.Fixture.DialogContext
                .Verify(c => c.Wait<IMessageActivity>(target.HandleActivityAsync));
        }

        [TestMethod]
        public async Task Resume_After_Child_Finishes()
        {
            var fromBot = this.Fixture.CreateMessage();

            var target = new Mock<RootDialog>(new Uri("http://license.com"), this.Fixture.TelemetryClient) { CallBase = true };

            await target.Object.ResumeAfterChildDialog(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(fromBot));

            target.Verify(d => d.HandleCommandAsync(this.Fixture.DialogContext.Object, It.IsAny<IMessageActivity>()), Times.Never);
        }

        [TestMethod]
        public async Task Resume_After_Child_UnknownCommandException()
        {
            var toBot = this.Fixture.CreateMessage();

            var awaitable = new Mock<IAwaitable<object>>();
            var awaiter = new Mock<IAwaiter<object>>();
            awaitable
                .Setup(a => a.GetAwaiter())
                .Returns(awaiter.Object);
            awaiter
                .Setup(a => a.IsCompleted)
                .Returns(true);
            awaiter
                .Setup(a => a.GetResult())
                .Throws<UnknownCommandException>();

            this.Fixture.DialogContext
                .Setup(c => c.Activity)
                .Returns(toBot);

            var target = new Mock<RootDialog>(new Uri("http://license.com"), this.Fixture.TelemetryClient) { CallBase = true };

            target
                .Setup(d => d.HandleCommandAsync(It.IsAny<IDialogContext>(), It.IsAny<IMessageActivity>()))
                .Returns(Task.CompletedTask);

            await target.Object.ResumeAfterChildDialog(this.Fixture.DialogContext.Object, awaitable.Object);

            target.Verify();
        }

        [TestMethod]
        public async Task Resume_After_Child_Exception()
        {
            var toBot = this.Fixture.CreateMessage();

            var awaitable = new Mock<IAwaitable<object>>();
            var awaiter = new Mock<IAwaiter<object>>();
            awaitable
                .Setup(a => a.GetAwaiter())
                .Returns(awaiter.Object);
            awaiter
                .Setup(a => a.IsCompleted)
                .Returns(true);
            awaiter
                .Setup(a => a.GetResult())
                .Throws<Exception>();

            this.Fixture.DialogContext
                .Setup(c => c.Activity)
                .Returns(toBot);

            this.Fixture.DialogContext
                .Setup(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None))
                .Returns(Task.CompletedTask);

            var target = new Mock<RootDialog>(new Uri("http://license.com"), this.Fixture.TelemetryClient) { CallBase = true };

            await target.Object.ResumeAfterChildDialog(this.Fixture.DialogContext.Object, awaitable.Object);

            this.Fixture.DialogContext.Verify();
        }

        [TestMethod]
        public async Task Handle_Message_Activity()
        {
            var message = this.Fixture.CreateMessage();

            // ReSharper disable once NotAccessedVariable
            var teamProject = "anteamproject";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var mocked = new Mock<RootDialog>(new Uri("https://an.url.toLicense"), this.Fixture.TelemetryClient) { CallBase = true };
            var target = mocked.Object;

            mocked.Setup(m => m.HandleCommandAsync(this.Fixture.DialogContext.Object, message)).Returns(Task.CompletedTask).Verifiable();

            await target.HandleActivityAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(message));

            mocked.Verify();
        }

        [TestMethod]
        public async Task Handle_Update_Activity()
        {
            var message = this.Fixture.CreateMessage();
            message.Type = ActivityTypes.ConversationUpdate;

            var mocked = new Mock<RootDialog>(new Uri("https://an.url.toLicense"), this.Fixture.TelemetryClient) { CallBase = true };
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