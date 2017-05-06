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
        private const string CommandMatch = "(connect)? *(\\w*) *(\\w*)";

        private readonly string appId;
        private readonly string authorizeUrl;

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
            var profile = context.UserData.GetCurrentProfile();
            var profiles = context.UserData.GetProfiles();
            var reply = context.MakeMessage();

            var match = Regex.Match(activity.Text.ToLowerInvariant(), CommandMatch);
            if (!match.Success)
            {
                return;
            }

            var account = match.Groups[1].Value;
            var teamProject = match.Groups[2].Value;

            if (!profiles.Any())
            {
                await this.Login(context, activity, reply);
            }

            if (string.IsNullOrWhiteSpace(account))
            {
                await this.SelectAccount(context, profiles, reply);
            }

            context.UserData.SetCurrentAccount(account);

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                // Select Team Project.
            }

            context.UserData.SetCurrentTeamProject(teamProject);

            if (profile != null)
            {
                context.UserData.SetCurrentProfile(profile);
                reply.Text = string.Format(Labels.ConnectedTo, account);
            }

            await context.PostAsync(reply);

            context.Done(reply);
        }

        private async Task Login(IDialogContext context, IMessageActivity activity, IMessageActivity reply)
        {
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

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            await context.PostAsync(reply);
            context.Wait(this.MessageReceivedAsync);
        }
    }
}