// ———————————————————————————————
// <copyright file="ConnectDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the dialog for connecting to a VSTS account and team project.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog for connecting to a VSTS account and team project.
    /// </summary>
    [CommandMetadata("connect")]
    [Serializable]
    public class ConnectDialog : IDialog<object>
    {
        private const string CommandMatchConnect = @"connect *(\w*) *(\w*)";
        private const string CommandMatchPin = @"(\d{4})";

        private readonly string appId;
        private readonly string authorizeUrl;

        [NonSerialized]
        private IVstsService vstsService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
        /// </summary>
        /// <param name="appId">The registered application id.</param>
        /// <param name="authorizeUrl">The URL to return to after authentication.</param>
        /// <param name="vstsService">VSTS accessor</param>
        public ConnectDialog(string appId, Uri authorizeUrl, IVstsService vstsService)
        {
            if (string.IsNullOrWhiteSpace(appId))
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (authorizeUrl == null)
            {
                throw new ArgumentNullException(nameof(authorizeUrl));
            }

            if (vstsService == null)
            {
                throw new ArgumentNullException(nameof(vstsService));
            }

            this.appId = appId;
            this.authorizeUrl = authorizeUrl.ToString();
            this.vstsService = vstsService;
        }

        /// <summary>
        /// Gets or sets an account.
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets a pin.
        /// </summary>
        public string Pin { get; set; }

        /// <summary>
        /// Gets or sets a <see cref="VstsProfile"/>.
        /// </summary>
        public VstsProfile Profile { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="VstsProfile"/>.
        /// </summary>
        public IEnumerable<VstsProfile> Profiles { get; set; }

        /// <summary>
        /// Gets or sets a team project.
        /// </summary>
        public string TeamProject { get; set; }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.ConnectAsync);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Connects the bot to a vsts account and team project.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">A <see cref="IMessageActivity"/>/</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task ConnectAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
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

            var match = Regex.Match(text, CommandMatchConnect);
            if (match.Success)
            {
                this.Account = match.Groups[1].Value;
                this.TeamProject = match.Groups[2].Value;
            }

            await this.ContinueProcess(context, activity);
        }

        /// <summary>
        /// Generates a login card and presents it to the user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="activity">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task LogOnAsync(IDialogContext context, IMessageActivity activity)
        {
            // Set pin.
            this.Pin = GeneratePin();
            context.UserData.SetPin(this.Pin);

            var card = new LogOnCard(this.appId, new Uri(this.authorizeUrl), activity.ChannelId, activity.From.Id);

            var reply = context.MakeMessage();
            reply.Attachments.Add(card);

            await context.PostAsync(reply);
            context.Wait(this.PinReceivedAsync);
        }

        /// <summary>
        /// Handles a received pin.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task PinReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();
            var match = Regex.Match(text, CommandMatchPin);

            if (match.Success && string.Equals(this.Pin, text, StringComparison.OrdinalIgnoreCase))
            {
                var profile = context.UserData.GetNotValidatedByPinProfile();
                var profiles = context.UserData.GetProfiles();

                profiles.Add(profile);
                this.Profile = profile;
                context.UserData.SetProfile(profile);

                await this.ContinueProcess(context, activity);
                return;
            }

            await context.PostAsync(Exceptions.InvalidPin);
            context.Wait(this.PinReceivedAsync);
        }

        /// <summary>
        /// Shows a selection list of accounts to the user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="activity">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task SelectAccountAsync(IDialogContext context, IMessageActivity activity)
        {
            var reply = context.MakeMessage();

            var accounts = this.Profiles
                .SelectMany(a => a.Accounts)
                .Distinct()
                .OrderBy(a => a)
                .ToArray();

            reply.Text = Labels.ConnectToAccount;
            reply.Attachments.Add(new AccountsCard(accounts));

            await context.PostAsync(reply);
            context.Wait(this.AccountReceivedAsync);
        }

        /// <summary>
        /// Handles the account selection.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task AccountReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            this.Account = activity.Text.Trim();
            this.Profile = this.Profiles
                .FirstOrDefault(p => p.Accounts.Any(a => string.Equals(a, this.Account, StringComparison.OrdinalIgnoreCase)));

            if (this.Profile == null)
            {
                await this.LogOnAsync(context, activity);
                return;
            }

            context.UserData.SetAccount(this.Account);
            context.UserData.SetProfile(this.Profile);

            await this.ContinueProcess(context, activity);
        }

        /// <summary>
        /// Shows a selection list of the projects to the user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="activity">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task SelectProjectAsync(IDialogContext context, IMessageActivity activity)
        {
            var reply = context.MakeMessage();

            var projects = await this.vstsService.GetProjects(this.Account, this.Profile.Token);
            var projectsNames = projects
                .Select(project => project.Name)
                .ToList();

            reply.Text = Labels.ConnectToProject;
            reply.Attachments.Add(new ProjectsCard(projectsNames));

            await context.PostAsync(reply);
            context.Wait(this.ProjectReceivedAsync);
        }

        /// <summary>
        /// Handles the team project selection.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task ProjectReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;

            this.TeamProject = activity.Text.Trim();

            context.UserData.SetTeamProject(this.TeamProject);

            await this.ContinueProcess(context, activity);
        }

        /// <summary>
        /// Continues the process.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="activity">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task ContinueProcess(IDialogContext context, IMessageActivity activity)
        {
            this.Profile = context.UserData.GetProfile();
            this.Profiles = context.UserData.GetProfiles();

            var reply = context.MakeMessage();

            // No Profiles, so we have to login.
            if (!this.Profiles.Any() || this.Profile == null)
            {
                await this.LogOnAsync(context, activity);
                return;
            }

            // No account, show a list available accounts.
            if (string.IsNullOrWhiteSpace(this.Account))
            {
                await this.SelectAccountAsync(context, activity);
                return;
            }

            // No team project, ....
            if (string.IsNullOrWhiteSpace(this.TeamProject))
            {
                await this.SelectProjectAsync(context, activity);
                return;
            }

            reply.Text = string.Format(Labels.ConnectedTo, this.Account, this.TeamProject);
            await context.PostAsync(reply);

            context.Done(reply);
        }

        private static string GeneratePin()
        {
            using (var generator = new RNGCryptoServiceProvider())
            {
                var data = new byte[4];

                generator.GetBytes(data);

                // Get the 5 significant numbers.
                var value = BitConverter.ToUInt32(data, 0) % 100000;

                return value.ToString("00000", CultureInfo.InvariantCulture);
            }
        }

        [ExcludeFromCodeCoverage]
        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
        }
    }
}