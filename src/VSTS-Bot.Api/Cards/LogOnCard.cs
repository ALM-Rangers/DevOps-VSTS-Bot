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
    using Microsoft.VisualStudio.Services.Common;
    using Resources;

    /// <summary>
    /// Represents the card that is used to logon.
    /// </summary>
    public class LogOnCard : HeroCard
    {
        private const string UrlOAuth = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state={1};{2}&scope={3}&redirect_uri={4}";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogOnCard"/> class.
        /// </summary>
        /// <param name="appId">The app id.</param>
        /// <param name="appScope">The app scope.</param>
        /// <param name="authorizeUrl">The authorizeUrl.</param>
        /// <param name="channelId">The channelId.</param>
        /// <param name="userId">The userId.</param>
        public LogOnCard(string appId, string appScope, Uri authorizeUrl, string channelId, string userId)
        {
            appId.ThrowIfNullOrWhiteSpace(nameof(appId));
            appScope.ThrowIfNullOrWhiteSpace(nameof(appScope));
            authorizeUrl.ThrowIfNull(nameof(authorizeUrl));
            channelId.ThrowIfNullOrWhiteSpace(nameof(channelId));
            userId.ThrowIfNullOrWhiteSpace(nameof(userId));

            this.Subtitle = Labels.PleaseLogin;

            var button = new CardAction
            {
                Value = string.Format(CultureInfo.InvariantCulture, UrlOAuth, appId, channelId, userId, appScope, authorizeUrl),
                Type = string.Equals(channelId, ChannelIds.Msteams, StringComparison.Ordinal) ? ActionTypes.OpenUrl : ActionTypes.Signin,
                Title = Labels.AuthenticationRequired
            };

            this.Buttons = new List<CardAction> { button };
        }
    }
}