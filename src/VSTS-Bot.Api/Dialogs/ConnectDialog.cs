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
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog for connecting to a VSTS account and team project.
    /// </summary>
    [CommandMetadata("connect")]
    [Serializable]
    public class ConnectDialog : DialogBase, IDialog<object>
    {
        private const string CommandMatchConnect = @"connect *(\S*) *(\S*)";
        private const string CommandMatchPin = @"(\d{4})";
        private const int TakeSize = 7;

        private readonly string appId;
        private readonly string appScope;
        private readonly string authorizeUrl;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
        /// </summary>
        /// <param name="appId">The registered application id.</param>
        /// <param name="appScope">The registered application scope.</param>
        /// <param name="authorizeUrl">The URL to return to after authentication.</param>
        /// <param name="authenticationService">The authenticationService.</param>
        /// <param name="vstsService">The <see cref="IVstsService"/>.</param>
        public ConnectDialog(string appId, string appScope, Uri authorizeUrl, IAuthenticationService authenticationService, IVstsService vstsService)
            : base(authenticationService, vstsService)
        {
            appId.ThrowIfNullOrWhiteSpace(nameof(appId));
            appScope.ThrowIfNullOrWhiteSpace(nameof(appScope));
            authorizeUrl.ThrowIfNull(nameof(authorizeUrl));

            this.appId = appId;
            this.appScope = appScope;
            this.authorizeUrl = authorizeUrl.ToString();
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
        /// Gets or sets a <see cref="TSBot.Profile"/>.
        /// </summary>
        public Profile Profile { get; set; }

        /// <summary>
        /// Gets or sets a collection of <see cref="TSBot.Profile"/>.
        /// </summary>
        public IEnumerable<Profile> Profiles { get; set; }

        /// <summary>
        /// Gets or sets a team project.
        /// </summary>
        public string TeamProject { get; set; }

        /// <summary>
        /// Gets or sets a list of team projects.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Need to be able to set the Team Projects.")]
        public IList<string> TeamProjects { get; set; } = new List<string>();

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.ThrowIfNull(nameof(context));

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
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;
            var text = (activity.Text ?? string.Empty).Trim().ToLowerInvariant();

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
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task LogOnAsync(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            if (!context.UserData.TryGetValue("userData", out UserData data))
            {
                data = new UserData();
            }

            // Set pin.
            this.Pin = GeneratePin();
            data.Pin = this.Pin;

            context.UserData.SetValue("userData", data);

            var card = new LogOnCard(this.appId, this.appScope, new Uri(this.authorizeUrl), result.ChannelId, result.From.Id);

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
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            var text = (activity.RemoveRecipientMention() ?? string.Empty).Trim().ToLowerInvariant();
            if (text.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                var reply = context.MakeMessage();
                reply.Text = Labels.HelpPin;
                await context.PostAsync(reply);
                context.Wait(this.PinReceivedAsync);
                return;
            }

            var match = Regex.Match(text, CommandMatchPin);

            if (match.Success && string.Equals(this.Pin, text, StringComparison.OrdinalIgnoreCase))
            {
                var data = context.UserData.GetValue<UserData>("userData");

                this.Profile = data.ProfileNotValidated;
                data.SelectProfile(this.Profile);
                data.Pin = string.Empty;

                context.UserData.SetValue("userData", data);

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
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task SelectAccountAsync(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var reply = context.MakeMessage();

            var accounts = this.Profiles
                .SelectMany(a => a.Accounts)
                .Distinct()
                .OrderBy(a => a)
                .ToList();

            reply.Text = Labels.ConnectToAccount;
            await context.PostAsync(reply);
            reply.Text = string.Empty;

            var skip = 0;
            while (skip < accounts.Count)
            {
                reply = context.MakeMessage();
                var local = accounts.Skip(skip).Take(TakeSize).ToList();
                reply.Attachments.Add(new AccountsCard(local));
                await context.PostAsync(reply);

                skip += TakeSize;
            }

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
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;

            var text = activity.RemoveRecipientMention().Trim();

            if (text.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                var reply = context.MakeMessage();
                reply.Text = Labels.HelpAccount;
                await context.PostAsync(reply);
                context.Wait(this.AccountReceivedAsync);
                return;
            }

            this.Account = text;

            this.Profile = this.Profiles.FirstOrDefault(p => p.Accounts.Any(
                a => string.Equals(a, this.Account, StringComparison.OrdinalIgnoreCase)));

            if (this.Profile == null)
            {
                var reply = context.MakeMessage();
                reply.Text = Labels.UnknownAccount;
                await context.PostAsync(reply);

                await this.SelectAccountAsync(context, activity);
                return;
            }

            var data = context.UserData.GetValue<UserData>("userData");

            data.Account = this.Account;
            data.SelectProfile(this.Profile);

            context.UserData.SetValue("userData", data);

            await this.ContinueProcess(context, activity);
        }

        /// <summary>
        /// Shows a selection list of the projects to the user.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/></param>
        /// <param name="result">A <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task SelectProjectAsync(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var reply = context.MakeMessage();

            var projects = await this.VstsService.GetProjects(this.Account, this.Profile.Token);
            this.TeamProjects = projects
                .Select(project => project.Name)
                .ToList();

            reply.Text = Labels.ConnectToProject;
            await context.PostAsync(reply);
            reply.Text = string.Empty;

            var skip = 0;
            while (skip < this.TeamProjects.Count)
            {
                reply = context.MakeMessage();
                var teamProjects = this.TeamProjects.Skip(skip).Take(TakeSize).ToList();
                reply.Attachments.Add(new ProjectsCard(teamProjects));
                await context.PostAsync(reply);

                skip += TakeSize;
            }

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
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            var activity = await result;
            var text = activity.RemoveRecipientMention().Trim();

            if (text.Equals("help", StringComparison.OrdinalIgnoreCase))
            {
                var reply = context.MakeMessage();
                reply.Text = Labels.HelpProject;
                await context.PostAsync(reply);
                context.Wait(this.ProjectReceivedAsync);
                return;
            }

            if (!this.TeamProjects.Contains(text))
            {
                await this.SelectProjectAsync(context, activity);
            }
            else
            {
                var data = context.UserData.GetValue<UserData>("userData");

                data.TeamProject = this.TeamProject = text;

                context.UserData.SetValue("userData", data);

                await this.ContinueProcess(context, activity);
            }
        }

        /// <summary>
        /// Continues the process.
        /// </summary>
        /// <param name="context">A <see cref="IDialogContext"/>.</param>
        /// <param name="result">An <see cref="IMessageActivity"/>.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public virtual async Task ContinueProcess(IDialogContext context, IMessageActivity result)
        {
            context.ThrowIfNull(nameof(context));
            result.ThrowIfNull(nameof(result));

            if (!context.UserData.TryGetValue("userData", out UserData data))
            {
                data = new UserData();
            }

            this.Profile = await this.GetValidatedProfile(context.UserData);
            this.Profiles = data.Profiles;

            var reply = context.MakeMessage();

            // No Profiles, so we have to login.
            if (!this.Profiles.Any() || this.Profile == null)
            {
                await this.LogOnAsync(context, result);
                return;
            }

            // No account, show a list available accounts.
            if (string.IsNullOrWhiteSpace(this.Account))
            {
                await this.SelectAccountAsync(context, result);
                return;
            }

            data.Account = this.Account;

            // No team project, ....
            if (string.IsNullOrWhiteSpace(this.TeamProject))
            {
                await this.SelectProjectAsync(context, result);
                return;
            }

            data.TeamProject = this.TeamProject;

            context.UserData.SetValue("userData", data);

            reply.Text = string.Format(Labels.ConnectedTo, result.From.Name, this.Account, this.TeamProject);
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
    }
}