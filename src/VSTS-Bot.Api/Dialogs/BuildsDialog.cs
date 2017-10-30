// ———————————————————————————————
// <copyright file="BuildsDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog to list/queue builds.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog to list/queue builds.
    /// </summary>
    [CommandMetadata("builds")]
    [Serializable]
    public class BuildsDialog : DialogBase, IDialog<object>
    {
        private const int TakeSize = 7;

        private const string CommandMatchBuilds = "builds";
        private const string CommandMatchQueue = @"queue (\d+)";

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildsDialog"/> class.
        /// </summary>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        public BuildsDialog(IAuthenticationService authenticationService, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
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
            context.ThrowIfNull(nameof(context));

            context.Wait(this.BuildsAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of builds.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task BuildsAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            this.Account = context.UserData.GetAccount();
            this.Profile = context.UserData.GetProfile(this.AuthenticationService);
            this.TeamProject = context.UserData.GetTeamProject();

            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();

            if (text.Equals(CommandMatchBuilds, StringComparison.OrdinalIgnoreCase))
            {
                var typing = context.MakeMessage();
                typing.Type = ActivityTypes.Typing;
                await context.PostAsync(typing);

                var buildDefinitions =
                    await this.VstsService.GetBuildDefinitionsAsync(this.Account, this.TeamProject, this.Profile.Token);
                if (!buildDefinitions.Any())
                {
                    var reply = context.MakeMessage();
                    reply.Text = Labels.NoBuilds;
                    await context.PostAsync(reply);
                    context.Done(reply);
                    return;
                }

                var skip = 0;
                while (skip < buildDefinitions.Count)
                {
                    var cards = buildDefinitions.Skip(skip).Take(TakeSize).Select(bd => new BuildDefinitionCard(bd)).ToList();
                    var reply = context.MakeMessage();

                    foreach (var card in cards)
                    {
                        reply.Attachments.Add(card);
                    }

                    reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                    await context.PostAsync(reply);

                    skip += TakeSize;
                }

                context.Wait(this.QueueAsync);
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }

        /// <summary>
        /// Queues a build.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task QueueAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;
            var text = (activity.RemoveRecipientMention() ?? string.Empty).ToLowerInvariant();
            var reply = context.MakeMessage();

            var match = Regex.Match(text, CommandMatchQueue);
            if (match.Success)
            {
                var typing = context.MakeMessage();
                typing.Type = ActivityTypes.Typing;
                await context.PostAsync(typing);

                var buildDefinitionId = Convert.ToInt32(match.Groups[1].Value);

                var build = await this.VstsService.QueueBuildAsync(this.Account, this.TeamProject, buildDefinitionId, this.Profile.Token);
                reply.Text = string.Format(Labels.BuildQueued, build.Id);

                await context.PostAsync(reply);
                context.Done(reply);
            }
            else
            {
                context.Fail(new UnknownCommandException(activity.Text));
            }
        }
    }
}