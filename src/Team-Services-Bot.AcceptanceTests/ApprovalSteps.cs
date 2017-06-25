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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Newtonsoft.Json.Linq;
    using TechTalk.SpecFlow;

    [Binding]
    public class ApprovalSteps
    {
        [Given(@"No approvals are waiting in '(.*)'")]
        public void GivenNoApprovalsAreWaitingIn(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();
            var approvals = service.GetApprovals(Config.Account, teamProject.Value, Config.Profile).Result;

            foreach (var approval in approvals)
            {
                service.ChangeApprovalStatus(Config.Account, teamProject.Value, Config.Profile, approval.Id, ApprovalStatus.Rejected, string.Empty).Wait();
            }
        }

        [Then(@"I get a list of approvals")]
        public void ThenIGetAListOfApprovalsOn(Table table)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.LastOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));
            var cards = activity.Attachments.Select(a => a.Content).Cast<JObject>().ToList();

            cards.Should().HaveCount(table.RowCount);

            for (var index = 0; index < cards.Count; index++)
            {
                var row = table.Rows[index];

                var card = cards[index];

                card["text"].Value<string>().Should().Be(row["Environment"]);
                card["title"].Value<string>().Should().Be(row["Release Definition"]);
            }
        }

        [Given(@"I have an approval for '(.*)', Release: '(.*)'")]
        public void GivenIHaveAnApprovalForRelease(KeyValuePair<string, string> teamProject, int releaseDefinitionId)
        {
            var service = new VstsService();
            var approvals = service.GetApprovals(Config.Account, teamProject.Value, Config.Profile).Result;
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
        public void ThenIsApprovedWithComment(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();
            var approval = service.GetApproval(Config.Profile.Token, Config.Account, teamProject.Value, Config.Approval.Id).Result;

            approval.Status.Should().Be(ApprovalStatus.Approved);
        }

        [Then(@"the approval is rejected for '(.*)'")]
        public void ThenIsRejectedWithComment(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();
            var approval = service.GetApproval(Config.Profile.Token, Config.Account, teamProject.Value, Config.Approval.Id).Result;

            approval.Status.Should().Be(ApprovalStatus.Rejected);
        }
    }
}