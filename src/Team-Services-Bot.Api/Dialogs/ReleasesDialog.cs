// ———————————————————————————————
// <copyright file="ReleasesDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog to list/create releases.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
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
    /// Represents the dialog to list/create releases.
    /// </summary>
    [CommandMetadata("releases")]
    [Serializable]
    public class ReleasesDialog : IDialog<object>
    {
        private const string CommandMatchReleases = "releases";
        private const string CommandMatchCreate = @"create (\d+)";

        [NonSerialized]
        private IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReleasesDialog"/> class.
        /// </summary>
        /// <param name="vstsService">VSTS accessor</param>
        /// <exception cref="ArgumentNullException">Occurs when the vstsService is missing.</exception>
        public ReleasesDialog(IVstsService vstsService)
        {
            this.vstsService = vstsService ?? throw new ArgumentNullException(nameof(vstsService));
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
            context.Wait(this.ReleasesAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Gets a list of release definitions.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task ReleasesAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            this.Account = context.UserData.GetAccount();
            this.Profile = context.UserData.GetProfile();
            this.TeamProject = context.UserData.GetTeamProject();

            var activity = await result;
            var text = (activity.Text ?? string.Empty).ToLowerInvariant();
            var reply = context.MakeMessage();

            if (text.Equals(CommandMatchReleases, StringComparison.OrdinalIgnoreCase))
            {
                var releaseDefinitions =
                    await this.vstsService.GetReleaseDefinitionsAsync(this.Account, this.TeamProject, this.Profile.Token);
                var cards = releaseDefinitions.Select(rd => new ReleaseDefinitionCard(rd)).ToList();

                foreach (var card in cards)
                {
                    reply.Attachments.Add(card);
                }

                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
                await context.PostAsync(reply);

                context.Wait(this.CreateAsync);
            }
            else
            {
                context.Done(reply);
            }
        }

        /// <summary>
        /// Creates a new release for a release definition.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public async Task CreateAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var activity = await result;
            var text = (activity.Text ?? string.Empty).ToLowerInvariant();
            var reply = context.MakeMessage();

            var match = Regex.Match(text, CommandMatchCreate);
            if (match.Success)
            {
                var definitionId = Convert.ToInt32(match.Groups[1].Value);
                var release = await this.vstsService.CreateReleaseAsync(this.Account, this.TeamProject, definitionId, this.Profile.Token);
                reply.Text = string.Format(Labels.ReleaseCreated, release.Id);

                await context.PostAsync(reply);
            }

            context.Done(reply);
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
        }
    }
}