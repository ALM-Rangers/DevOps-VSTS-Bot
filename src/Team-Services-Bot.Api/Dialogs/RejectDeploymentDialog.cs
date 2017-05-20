// ———————————————————————————————
// <copyright file="RejectDeploymentDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a dialog for rejecting a deployment.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents a dialog for rejecting a deployment.
    /// </summary>
    [CommandMetadata("reject")]
    [Serializable]
    public class RejectDeploymentDialog : IDialog<object>
    {
        private const string CommandMatch = @"reject (\d+) *(.*?)$";

        [NonSerialized]
        private IVstsService vstsService;
        [NonSerialized]
        private IDialogContextWrapper wrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="RejectDeploymentDialog"/> class.
        /// </summary>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        /// <param name="wrapper">The wrapper around the <see cref="IDialogContext"/>.</param>
        public RejectDeploymentDialog(IVstsService vstsService, IDialogContextWrapper wrapper)
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

            var matchApprove = Regex.Match(activity.Text, CommandMatch);

            if (matchApprove.Success)
            {
                var approvalId = Convert.ToInt32(matchApprove.Groups[1].Value);
                var comment = matchApprove.Groups[2].Value;

                await this.vstsService.RejectDeployment(account, teamProject, profile, approvalId, comment);

                reply.Text = Labels.Rejected;

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