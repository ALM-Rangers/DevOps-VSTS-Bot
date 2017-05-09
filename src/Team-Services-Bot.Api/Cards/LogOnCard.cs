// ———————————————————————————————
// <copyright file="LogOnCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the login card.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Cards
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represent the login card.
    /// </summary>
    public class LogOnCard : SigninCard
    {
        private const string Scope = "vso.agentpools%20vso.build_execute%20vso.chat_write%20vso.code%20vso.connected_server%20" +
                                     "vso.dashboards%20vso.entitlements%20vso.extension%20vso.extension.data%20vso.gallery%20" +
                                     "vso.identity%20vso.loadtest%20vso.notification%20vso.packaging%20vso.project%20" +
                                     "vso.release_execute%20vso.serviceendpoint%20vso.taskgroups%20vso.test%20vso.work";

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
                Value = string.Format(UrlOAuth, appId, channelId, userId, Scope, authorizeUrl),
                Type = ActionTypes.Signin,
                Title = Labels.AuthenticationRequired
            };

            this.Buttons = new List<CardAction> { button };
        }
    }
}