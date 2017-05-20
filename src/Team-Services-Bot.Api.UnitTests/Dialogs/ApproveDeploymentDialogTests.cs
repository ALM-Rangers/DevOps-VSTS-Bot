// ———————————————————————————————
// <copyright file="ApproveDeploymentDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the ApproveDeploymentDialog.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System.Threading.Tasks;
    using Autofac;
    using Common.Tests;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ApproveDeploymentDialogTests : TestsBase<DialogFixture>
    {
        public ApproveDeploymentDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Approve_Approval()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approve 1 a comment";

            var account = "anaccount";
            var profile = new VstsProfile();
            var teamProject = "anteamproject";

            var service = new Mock<IVstsService>();

            var builder = new ContainerBuilder();
            builder
                .Register(c => service.Object)
                .As<IVstsService>();
            builder
                .RegisterType<ApproveDeploymentDialog>()
                .As<IDialog<object>>();

            var container = this.Fixture.Build(builder);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            this.Fixture.RootDialog.Initialized = true;

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            toUser.Text.Should().Be("The deployment is approved.");

            service
                .Verify(s => s.ApproveDeployment(account, teamProject, profile, 1, "a comment"));
        }
    }
}