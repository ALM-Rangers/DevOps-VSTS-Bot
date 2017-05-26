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
        private IDialogContextWrapper wrapper;

        [NonSerialized]
        private IVstsService vstsService;

        private string accountName;
        private bool isPinActivated;
        private string teamProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
        /// </summary>
        /// <param name="appId">The registered application id.</param>
        /// <param name="authorizeUrl">The URL to return to after authentication.</param>
        /// <param name="wrapper">The wrapper</param>
        /// <param name="vstsService">VSTS accessor</param>
        public ConnectDialog(string appId, Uri authorizeUrl, IDialogContextWrapper wrapper, IVstsService vstsService)
        {
            if (appId == null)
            {
                throw new ArgumentNullException(nameof(appId));
            }

            if (authorizeUrl == null)
            {
                throw new ArgumentNullException(nameof(authorizeUrl));
            }

            if (wrapper == null)
            {
                throw new ArgumentNullException(nameof(wrapper));
            }

            if (vstsService == null)
            {
                throw new ArgumentNullException(nameof(vstsService));
            }

            this.appId = appId;
            this.authorizeUrl = authorizeUrl.ToString();
            this.wrapper = wrapper;
            this.vstsService = vstsService;
        }

        /// <inheritdoc />
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));

            await Task.CompletedTask;
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

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result, IBotDataBag userData)
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
            var pin = userData.GetPin();
            var profile = userData.GetCurrentProfile();
            var profiles = userData.GetProfiles();
            var account = userData.GetCurrentAccount();
            var reply = context.MakeMessage();
            var text = (activity.Text ?? string.Empty).ToLowerInvariant();

            // Pin was previously created, waiting for it to return.
            if (this.isPinActivated)
            {
                var isPinHandled = await this.HandlePin(context, activity, pin);
                if (!isPinHandled)
                {
                    return;
                }
            }
            else
            {
                // Wait for the normal expected input.
                var match = Regex.Match(text, CommandMatchConnect);
                if (match.Success)
                {
                    this.accountName = match.Groups[1].Value;
                    this.teamProject = match.Groups[2].Value;
                }
            }

            // No Profiles, so we have to login.
            if (!profiles.Any() || profile == null)
            {
                await this.Login(context, activity, reply, userData);
                return;
            }

            // No account, show a list available accounts.
            if (string.IsNullOrWhiteSpace(this.accountName))
            {
                await this.SelectAccountAsync(context, profiles, reply);
                return;
            }

            if (!string.IsNullOrWhiteSpace(this.accountName) && profile.Accounts != null)
            {
                account = profile.Accounts.First(a => string.Equals(this.accountName, a.Name, StringComparison.OrdinalIgnoreCase));
            }

            // No team project, ....
            if (string.IsNullOrWhiteSpace(this.teamProject))
            {
                await this.SelectProjectAsync(context, account, reply);
                return;
            }

            userData.SetCurrentAccount(account);
            userData.SetCurrentTeamProject(this.teamProject);

            reply.Text = string.Format(Labels.ConnectedTo, this.accountName, this.teamProject);
            await context.PostAsync(reply);

            context.Done(reply);
        }

        private async Task<bool> HandlePin(IDialogContext context, IMessageActivity activity, string pin)
        {
            this.isPinActivated = false;

            var text = (activity.Text ?? string.Empty).ToLowerInvariant();
            var match = Regex.Match(text, CommandMatchPin);

            if (match.Success && string.Equals(pin, text, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            await context.PostAsync(Exceptions.InvalidPin);
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));

            return false;
        }

        private async Task Login(IDialogContext context, IMessageActivity activity, IMessageActivity reply, IBotDataBag userData)
        {
            // Set pin.
            var pin = GeneratePin();
            userData.SetPin(pin);
            this.isPinActivated = true;

            var card = new LogOnCard(this.appId, new Uri(this.authorizeUrl), activity.ChannelId, Labels.PleaseLogin, activity.From.Id);

            reply.Attachments.Add(card);

            await context.PostAsync(reply);
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));
        }

        [OnSerializing]
        private void OnSerializingMethod(StreamingContext context)
        {
            this.wrapper = GlobalConfiguration.Configuration.DependencyResolver.GetService<IDialogContextWrapper>();
            this.vstsService = GlobalConfiguration.Configuration.DependencyResolver.GetService<IVstsService>();
        }

        private async Task SelectAccountAsync(IDialogContext context, IList<VstsProfile> profiles, IMessageActivity reply)
        {
            string[] accounts = profiles.SelectMany(a => a.Accounts).Distinct().OrderBy(a => a).Select(vstsAccount => vstsAccount.Name).ToArray();

            await this.SelectAsync(context, reply, Labels.ConnectToAccount, new AccountsCard(accounts));
        }

        private async Task SelectProjectAsync(IDialogContext context, VstsAccount account, IMessageActivity reply)
        {
            var projects = await this.vstsService.GetProjects(account.Url, account.Token);
            var projectNames = projects.Select(project => project.Name);

            await this.SelectAsync(context, reply, Labels.ConnectToProject, new ProjectsCard(projectNames));
        }

        private async Task SelectAsync(IDialogContext context, IMessageActivity reply, string replyText, HeroCard card)
        {
            reply.Text = replyText;
            reply.Attachments.Add(card);

            await context.PostAsync(reply);
            context.Wait((c, result) => this.MessageReceivedAsync(c, result, this.wrapper.GetUserData(c)));
        }
    }
}