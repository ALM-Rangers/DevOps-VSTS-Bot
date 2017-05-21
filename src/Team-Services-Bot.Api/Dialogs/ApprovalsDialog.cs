// ———————————————————————————————
// <copyright file="ApprovalsDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog to retrieve and approve/reject approvals.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Resources;

    /// <summary>
    /// Represents the dialog to retrieve and approve/reject approvals.
    /// </summary>
    [CommandMetadata("approvals")]
    [Serializable]
    public class ApprovalsDialog : IDialog<object>
    {
        private const string CommandMatchApprovals = "approvals";
        private const string CommandMatchApprove = @"approve (\d+) *(.*?)$";
        private const string CommandMatchReject = @"reject (\d+) *(.*?)$";

        [NonSerialized]
        private IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalsDialog"/> class.
        /// </summary>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        public ApprovalsDialog(IVstsService vstsService)
        {
            this.vstsService = vstsService;
        }

        /// <summary>
        /// Gets or sets the account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the profile.
        /// </summary>
        public VstsProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets the Team Project.
        /// </summary>
        public string TeamProject { get; set; }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.ApprovalsAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Replies with a list of ApprovalCards.
        /// </summary>
        /// <param name="context">The <see cref="IDialogContext"/>.</param>
        /// <param name="result">The <see cref="IAwaitable{T}"/>.</param>
        /// <returns>An async <see cref="Task"/>/.</returns>
        public async Task ApprovalsAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var reply = context.MakeMessage();

            this.Account = context.UserData.GetCurrentAccount();
            this.Profile = context.UserData.GetProfile();
            this.TeamProject = context.UserData.GetCurrentTeamProject();

            if (activity.Text.Equals(CommandMatchApprovals, StringComparison.OrdinalIgnoreCase))
            {
                var approvals = await this.vstsService.GetApprovals(this.Account, this.TeamProject, this.Profile);
                var cards = approvals.Select(a => new ApprovalCard(a)).ToList();

                foreach (var card in cards)
                {
                    reply.Attachments.Add(card);
                }

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                await context.PostAsync(reply);

                context.Wait(this.ApproveOrReject);
            }
        }

        /// <summary>
        /// Approves or Rejects an Approval.
        /// </summary>
        /// <param name="context">The <see cref="IDialogContext"/>.</param>
        /// <param name="result">The <see cref="IAwaitable{T}"/>.</param>
        /// <returns>An async <see cref="Task"/>/.</returns>
        public async Task ApproveOrReject(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            var matchApprove = Regex.Match(activity.Text, CommandMatchApprove);
            var matchReject = Regex.Match(activity.Text, CommandMatchReject);

            var reply = context.MakeMessage();

            if (matchApprove.Success)
            {
                var approvalId = Convert.ToInt32(matchApprove.Groups[1].Value);
                var comment = matchApprove.Groups[2].Value;

                if (string.IsNullOrWhiteSpace(comment))
                {
                    reply.Text = Labels.MissingComment;
                    await context.PostAsync(reply);
                    context.Wait((c, r) => this.ChangeStatus(c, r, approvalId, true));
                }
                else
                {
                    await this.ChangeStatus(context, approvalId, comment, true);
                }
            }
            else if (matchReject.Success)
            {
                var approvalId = Convert.ToInt32(matchReject.Groups[1].Value);
                var comment = matchReject.Groups[2].Value;

                if (string.IsNullOrWhiteSpace(comment))
                {
                    reply.Text = Labels.MissingComment;
                    await context.PostAsync(reply);
                    context.Wait((c, r) => this.ChangeStatus(c, r, approvalId, false));
                }
                else
                {
                    await this.ChangeStatus(context, approvalId, comment, false);
                }
            }
        }

        private async Task ChangeStatus(IDialogContext context, IAwaitable<IMessageActivity> result, int approvalId, bool isApproval)
        {
            var activity = await result;

            await this.ChangeStatus(context, approvalId, activity.Text, isApproval);
        }

        private async Task ChangeStatus(IDialogContext context, int approvalId, string comment, bool isApproval)
        {
            var reply = context.MakeMessage();

            if (string.IsNullOrWhiteSpace(comment))
            {
                reply.Text = Labels.MissingComment;
                await context.PostAsync(reply);
            }

            var status = isApproval ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
            await this.vstsService.ChangeApprovalStatus(this.Account, this.TeamProject, this.Profile, approvalId, status, comment);

            reply.Text = isApproval ? Labels.Approved : Labels.Rejected;
            await context.PostAsync(reply);

            context.Done(reply);
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
        }
    }
}