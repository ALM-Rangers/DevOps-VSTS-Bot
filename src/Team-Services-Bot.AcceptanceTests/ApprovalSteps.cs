// ———————————————————————————————
// <copyright file="ApprovalSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the steps for approvals.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Cards;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using TechTalk.SpecFlow;

    [Binding]
    public class ApprovalSteps
    {
        [Given(@"No approvals are waiting in '(.*)'")]
        public async Task GivenNoApprovalsAreWaitingIn(string teamProject)
        {
            var service = new VstsService();
            var approvals = await service.GetApprovals(Config.Account, teamProject, Config.Profile);

            foreach (var approval in approvals)
            {
                await service.ChangeApprovalStatus(Config.Account, teamProject, Config.Profile, approval.Id, ApprovalStatus.Canceled, string.Empty);
            }
        }

        [Then(@"I get a list of approvals")]
        public void ThenIGetAListOfApprovalsOn(Table table)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.FirstOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));
            var cards = activity.Attachments.Select(a => a.Content).Cast<ApprovalCard>().ToList();

            for (var index = 0; index < cards.Count; index++)
            {
                var row = table.Rows[index];

                var card = cards[index];
                card.Text.Should().Be(row["Environment"]);
                card.Title.Should().Be(row["Release Definition"]);
            }
        }

        [Given(@"I have an approval for '(.*)', Release: '(.*)'")]
        public async Task GivenIHaveAnApprovalForRelease(string teamProject, int releaseDefinitionId)
        {
            var service = new VstsService();
            var approvals = await service.GetApprovals(Config.Account, teamProject, Config.Profile);
            Config.Approval = approvals.FirstOrDefault(a => a.ReleaseDefinitionReference.Id.Equals(releaseDefinitionId));
        }

        [When(@"I approve the approval with comment '(.*)'")]
        public void WhenIApproveTheApprovalWithComment(string comment)
        {
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount(Config.UserName, Config.UserName),
                Text = FormattableString.Invariant($"approve {Config.Approval.Id} {comment}")
            };

            Config.Client.Conversations.PostActivity(Config.ConversationId, activity);
        }

        [When(@"I reject the approval with comment '(.*)'")]
        public void WhenIRejectTheApprovalWithComment(string comment)
        {
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount(Config.UserName, Config.UserName),
                Text = FormattableString.Invariant($"reject {Config.Approval.Id} {comment}")
            };

            Config.Client.Conversations.PostActivity(Config.ConversationId, activity);
        }

        [Then(@"the approval is approved for '(.*)'")]
        public async Task ThenIsApprovedWithComment(string teamProject)
        {
            var service = new VstsService();
            var approval = await service.GetApproval(Config.Profile.Token, Config.Account, teamProject, Config.Approval.Id);

            approval.Status.Should().Be(ApprovalStatus.Approved);
        }

        [Then(@"the approval is rejected for '(.*)'")]
        public async Task ThenIsRejectedWithComment(string teamProject)
        {
            var service = new VstsService();
            var approval = await service.GetApproval(Config.Profile.Token, Config.Account, teamProject, Config.Approval.Id);

            approval.Status.Should().Be(ApprovalStatus.Rejected);
        }
    }
}