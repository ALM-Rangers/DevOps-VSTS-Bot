// ———————————————————————————————
// <copyright file="LogOnCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the card that is used to logon.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the card that is used to logon.
    /// </summary>
    public class LogOnCard : SigninCard
    {
        private const string Scope = "vso.agentpools_manage%20vso.build_execute%20vso.chat_manage%20vso.code_manage%20vso.code_status%20" +
                                     "vso.connected_server%20vso.dashboards%20vso.dashboards_manage%20vso.entitlements%20vso.extension.data_write%20" +
                                     "vso.extension_manage%20vso.gallery_acquire%20vso.gallery_manage%20vso.identity%20vso.loadtest_write%20" +
                                     "vso.notification_manage%20vso.packaging_manage%20vso.profile_write%20vso.project_manage%20vso.release_manage%20" +
                                     "vso.security_manage%20vso.serviceendpoint_manage%20vso.taskgroups_manage%20vso.test_write%20vso.work_write";

        private const string UrlOAuth = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state={1};{2}&scope={3}&redirect_uri={4}";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogOnCard"/> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="authorizeUrl">The authorizeUrl.</param>
        /// <param name="channelId">The channelId.</param>
        /// <param name="text">The text on the card.</param>
        /// <param name="userId">The userId.</param>
        public LogOnCard(string appId, Uri authorizeUrl, string channelId, string text, string userId)
            : base(text)
        {
            var button = new CardAction
            {
                Value = string.Format(CultureInfo.InvariantCulture, UrlOAuth, appId, channelId, userId, Scope, authorizeUrl),
                Type = string.Equals(channelId, ChannelIds.Msteams, StringComparison.Ordinal) ? ActionTypes.OpenUrl : ActionTypes.Signin,
                Title = Labels.AuthenticationRequired
            };

            this.Buttons = new List<CardAction> { button };
        }
    }
}