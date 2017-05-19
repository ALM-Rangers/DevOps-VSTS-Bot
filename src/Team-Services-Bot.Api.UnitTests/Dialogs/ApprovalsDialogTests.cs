// ———————————————————————————————
// <copyright file="ApprovalsDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the ApprovalsDialog.
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
    using Common.Tests;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class ApprovalsDialogTests : TestsBase<DialogFixture>
    {
        public ApprovalsDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task List_Approvals()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approvals";

            var account = "anaccount";
            var approval1 = new ReleaseApproval
            {
                ReleaseReference = new ReleaseShallowReference { Name = "Release 1", Url = "urlToRelease" },
                ReleaseDefinitionReference = new ReleaseDefinitionShallowReference { Name = "Release Definition 1" },
                ReleaseEnvironmentReference = new ReleaseEnvironmentShallowReference { Name = "Development" }
            };
            var approvals = new List<ReleaseApproval> { approval1 };
            var profile = new VstsProfile();
            var teamProject = "anteamproject";

            var service = new Mock<IVstsService>();

            var builder = new ContainerBuilder();
            builder
                .Register(c => service.Object)
                .As<IVstsService>();
            builder
                .RegisterType<ApprovalsDialog>()
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

            service
                .Setup(s => s.GetApprovals(account, teamProject, profile))
                .ReturnsAsync(approvals);

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            var attachment = toUser.Attachments.FirstOrDefault();
            attachment.Should().NotBeNull();

            var card = attachment.Content;

            card.Should().BeOfType<ApprovalCard>().Subject.Title.Should().Be(approval1.ReleaseDefinitionReference.Name);
            card.Should().BeOfType<ApprovalCard>().Subject.Subtitle.Should().Be(approval1.ReleaseReference.Name);
            card.Should().BeOfType<ApprovalCard>().Subject.Text.Should().Be(approval1.ReleaseEnvironmentReference.Name);
            card.Should().BeOfType<ApprovalCard>().Subject.Tap.Value.Should().Be(approval1.ReleaseReference.Url);
        }
    }
}