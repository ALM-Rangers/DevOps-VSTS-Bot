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
        private const string CommandMatch = "connect (.*?)$";
        private const string Scope = "vso.agentpools_manage%20vso.build_execute%20vso.chat_manage%20vso.code_manage%20vso.code_status%20" +
                                     "vso.connected_server%20vso.dashboards_manage%20vso.entitlements%20vso.extension.data_write%20" +
                                     "vso.extension_manage%20vso.gallery_acquire%20vso.gallery_manage%20vso.identity%20vso.loadtest_write%20" +
                                     "vso.notification_manage%20vso.packaging_manage%20vso.profile_write%20vso.project_manage%20" +
                                     "vso.release_manage%20vso.security_manage%20vso.serviceendpoint_manage%20vso.test_write%20vso.work_write";

        private const string UrlOAuth = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state={1};{2}&scope={3}&redirect_uri={4}";

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

            var match = Regex.Match(activity.Text.ToLowerInvariant(), CommandMatch);
            if (match.Success)
            {
                var account = match.Groups[1].Value;
                context.UserData.SetCurrentAccount(account);

                var reply = context.MakeMessage();
                var profiles = context.UserData.GetProfiles();
                var profile = profiles.FirstOrDefault(p => p.Accounts.Any(a => a.Equals(account, StringComparison.OrdinalIgnoreCase)));

                if (profile != null)
                {
                    context.UserData.SetCurrentProfile(profile);
                    reply.Text = string.Format(Labels.ConnectedTo, account);
                }
                else
                {
                    var button = new CardAction
                    {
                        Value = string.Format(
                            UrlOAuth,
                            this.appId,
                            activity.ChannelId,
                            activity.From.Id,
                            Scope,
                            this.authorizeUrl),
                        Type = activity.IsTeamsChannel() ? "openUrl" : "signin",
                        Title = Labels.AuthenticationRequired
                    };

                    var card = new SigninCard(Labels.PleaseLogin, new List<CardAction> { button });

                    reply.Attachments.Add(card);
                }

                await context.PostAsync(reply);

                context.Done(reply);
            }
        }
    }
}