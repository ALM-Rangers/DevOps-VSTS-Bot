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
    using System.Diagnostics.CodeAnalysis;
    using System.Threading;
    using System.Threading.Tasks;
    using Dialogs;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class ApprovalsDialogTests : TestsBase<DialogFixture>
    {
        public ApprovalsDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Missing_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApprovalsDialog(this.Fixture.AuthenticationService.Object, null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor_Missing_AuthenticationService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApprovalsDialog(null, this.Fixture.VstsService.Object));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Constructor()
        {
            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            Assert.IsNotNull(target);

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<ApprovalsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ApprovalsAsync));
        }

        [TestMethod]
        public async Task List_Approvals_Empty_List()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approvals";

            var approvals = new List<ReleaseApproval>();

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            this.Fixture.VstsService
                .Setup(s => s.GetApprovals(data.Account, data.TeamProject, profile))
                .ReturnsAsync(approvals);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await target.ApprovalsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
            this.Fixture.DialogContext
                .Verify(c => c.Done(It.IsAny<IMessageActivity>()));
        }

        [TestMethod]
        public async Task List_Approvals_Null_Message()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var profile = this.Fixture.CreateProfile();
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await target.ApprovalsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task List_Approvals()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approvals";

            var profile = this.Fixture.CreateProfile();
            profile.Token.ExpiresIn = 0;
            var data = new UserData { Account = "anaccount", TeamProject = "anteamproject" };
            data.Profiles.Add(profile);

            var approval = new ReleaseApproval
            {
                Id = 1,
                ReleaseReference = new ReleaseShallowReference { Name = "Release 1", Url = "urlToRelease" },
                ReleaseDefinitionReference = new ReleaseDefinitionShallowReference { Name = "Release Definition 1" },
                ReleaseEnvironmentReference = new ReleaseEnvironmentShallowReference { Name = "Development" }
            };
            var approvals = new List<ReleaseApproval> { approval };

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("userData", out data))
                .Returns(true);
            this.Fixture.VstsService
                .Setup(s => s.GetApprovals(data.Account, data.TeamProject, profile))
                .ReturnsAsync(approvals);
            this.Fixture.AuthenticationService
                .Setup(a => a.GetToken(profile.Token))
                .ReturnsAsync(new OAuthToken());

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await target.ApprovalsAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));

            this.Fixture.DialogContext
                .Verify(
                    c => c.Wait(
                        It.Is<ResumeAfter<IMessageActivity>>(a => a.Method == target.GetType().GetMethod("ApproveOrRejectAsync"))));
        }

        [TestMethod]
        public async Task Approve_Approval_Without_Comment()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approve 1";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.ApproveOrRejectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Equals("Please provide a comment.", StringComparison.OrdinalIgnoreCase)), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ChangeStatusAsync));
        }

        [TestMethod]
        public async Task Approve_Approval()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "approve 1 a comment";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.ApproveOrRejectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService
                .Verify(s => s.ChangeApprovalStatus(account, teamProject, profile, 1, ApprovalStatus.Approved, "a comment"));
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task Reject_Approval_Without_Comment()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "reject 1";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.ApproveOrRejectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.Is<IMessageActivity>(a => a.Text.Equals("Please provide a comment.", StringComparison.OrdinalIgnoreCase)), CancellationToken.None));
            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.ChangeStatusAsync));
        }

        [TestMethod]
        public async Task Reject_Approval()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "reject 1 a comment";

            var account = "anaccount";
            var profile = this.Fixture.CreateProfile();
            var teamProject = "anteamproject";

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object)
            {
                Account = account,
                Profile = profile,
                TeamProject = teamProject
            };

            await target.ApproveOrRejectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.VstsService
                .Verify(s => s.ChangeApprovalStatus(account, teamProject, profile, 1, ApprovalStatus.Rejected, "a comment"));
            this.Fixture.DialogContext
                .Verify(c => c.PostAsync(It.IsAny<IMessageActivity>(), CancellationToken.None));
        }

        [TestMethod]
        public async Task Approval_Invalid_Message()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "invalid";

            var target = new ApprovalsDialog(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object);

            await target.ApproveOrRejectAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            this.Fixture.DialogContext.Verify(c => c.Fail(It.IsAny<UnknownCommandException>()));
        }

        [TestMethod]
        public async Task Change_Status()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "A comment";

            var mocked = new Mock<ApprovalsDialog>(this.Fixture.AuthenticationService.Object, this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;
            target.ApprovalId = 1;
            target.IsApproved = true;

            mocked.Setup(m => m.ChangeStatusAsync(this.Fixture.DialogContext.Object, 1, toBot.Text, true)).Returns(Task.CompletedTask).Verifiable();

            await target.ChangeStatusAsync(this.Fixture.DialogContext.Object, this.Fixture.MakeAwaitable(toBot));

            mocked.Verify();
        }
    }
}