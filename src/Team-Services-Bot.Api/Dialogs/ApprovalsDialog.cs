﻿// ———————————————————————————————
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
    using Resources;

    /// <summary>
    /// Represents the dialog to retrieve and approve/reject approvals.
    /// </summary>
    [CommandMetadata("approvals")]
    [Serializable]
    public class ApprovalsDialog : IDialog<object>
    {
        private const string CommandMatchApprovals = "approvals";

        [NonSerialized]
        private IVstsService vstsService;
        [NonSerialized]
        private IDialogContextWrapper wrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalsDialog"/> class.
        /// </summary>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        /// <param name="wrapper">The wrapper around the <see cref="IDialogContext"/>.</param>
        public ApprovalsDialog(IVstsService vstsService, IDialogContextWrapper wrapper)
        {
            this.vstsService = vstsService;
            this.wrapper = wrapper;
        }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));

            await Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result, IBotDataBag userData)
        {
            var activity = await result;

            var account = userData.GetCurrentAccount();
            var profile = userData.GetProfile();
            var reply = context.MakeMessage();
            var teamProject = userData.GetCurrentTeamProject();

            if (activity.Text.Equals(CommandMatchApprovals, StringComparison.OrdinalIgnoreCase))
            {
                var approvals = await this.vstsService.GetApprovals(account, teamProject, profile);
                var cards = approvals.Select(a => new ApprovalCard(a)).ToList();

                foreach (var card in cards)
                {
                    reply.Attachments.Add(card);
                }

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                await context.PostAsync(reply);
            }
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
            this.wrapper = GlobalConfiguration.Configuration.DependencyResolver.GetService<IDialogContextWrapper>();
        }
    }
}