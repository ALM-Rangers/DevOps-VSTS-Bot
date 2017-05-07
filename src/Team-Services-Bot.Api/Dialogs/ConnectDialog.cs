// ———————————————————————————————
// <copyright file="ConnectDialog.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the Root Dialog logic to handle messages.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Cards;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the dialog to connect to an account.
    /// </summary>
    [CommandMetadata("connect")]
    [Serializable]
    public class ConnectDialog : IDialog<object>
    {
        private const string CommandMatchConnect = "connect *(\\w*) *(\\w*)";
        private const string CommandMatchPin = "(\\d{4})";
        private const int Min = 0;
        private const int Max = 9999;

        private readonly string appId;
        private readonly string authorizeUrl;

        private string account;
        private bool isPinActivated;
        private string teamProject;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectDialog"/> class.
        /// </summary>
        /// <param name="appId">The registered application id.</param>
        /// <param name="authorizeUrl">The url to return to after authentication.</param>
        public ConnectDialog(string appId, Uri authorizeUrl)
        {
            if (authorizeUrl == null)
            {
                throw new ArgumentNullException(nameof(authorizeUrl));
            }

            this.appId = appId;
            this.authorizeUrl = authorizeUrl.ToString();
        }

        /// <inheritdoc />
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result;
            var pin = context.UserData.GetPin();
            var profile = context.UserData.GetCurrentProfile();
            var profiles = context.UserData.GetProfiles();
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
                    this.account = match.Groups[2].Value;
                    this.teamProject = match.Groups[3].Value;
                }
            }

            // No Profiles, so we have to login.
            if (!profiles.Any() || profile == null)
            {
                await this.Login(context, activity, reply);
                return;
            }

            // No account, show a list avaiable accounts.
            if (string.IsNullOrWhiteSpace(this.account))
            {
                await this.SelectAccount(context, profiles, reply);
                return;
            }

            // No team project, ....
            if (string.IsNullOrWhiteSpace(this.teamProject))
            {
                // Select Team Project.
            }

            context.UserData.SetCurrentAccount(this.account);
            context.UserData.SetCurrentTeamProject(this.teamProject);

            reply.Text = string.Format(Labels.ConnectedTo, this.account);
            await context.PostAsync(reply);

            context.Done(reply);
        }

        private string GeneratePin()
        {
            var generator = new Random();
            return generator.Next(Min, Max).ToString("0000");
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
            context.Wait(this.MessageReceivedAsync);

            return false;
        }

        private async Task Login(IDialogContext context, IMessageActivity activity, IMessageActivity reply)
        {
            // Set pin.
            var pin = this.GeneratePin();
            context.UserData.SetPin(pin);
            this.isPinActivated = true;

            var card = new LoginCard(this.appId, this.authorizeUrl, activity.ChannelId, Labels.PleaseLogin, activity.From.Id);

            reply.Attachments.Add(card);

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }

        private async Task SelectAccount(IDialogContext context, IList<VstsProfile> profiles, IMessageActivity reply)
        {
            reply.Text = Labels.ConnectToAccount;
            foreach (var acc in profiles.SelectMany(a => a.Accounts).Distinct().OrderBy(a => a))
            {
                reply.Attachments.Add(new AccountCard(acc));
            }

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }
    }
}