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
        private const string UrlOAuth = "https://app.vssps.visualstudio.com/oauth2/authorize?client_id={0}&response_type=Assertion&state={1};{2}&scope={3}&redirect_uri={4}";

        /// <summary>
        /// Initializes a new instance of the <see cref="LogOnCard"/> class.
        /// </summary>
        /// <param name="application">VSTS application information</param>
        /// <param name="authorizeUrl">The authorizeUrl.</param>
        /// <param name="channelId">The channelId.</param>
        /// <param name="userId">The userId.</param>
        public LogOnCard(VstsApplication application, Uri authorizeUrl, string channelId, string userId)
            : base(Labels.PleaseLogin)
        {
            application.ThrowIfNull(nameof(application));
            authorizeUrl.ThrowIfNull(nameof(authorizeUrl));
            channelId.ThrowIfNullOrWhiteSpace(nameof(channelId));
            userId.ThrowIfNullOrWhiteSpace(nameof(userId));

            var button = new CardAction
            {
                Value = string.Format(CultureInfo.InvariantCulture, UrlOAuth, application.Id, channelId, userId, application.Scope, authorizeUrl),
                Type = string.Equals(channelId, ChannelIds.Msteams, StringComparison.Ordinal) ? ActionTypes.OpenUrl : ActionTypes.Signin,
                Title = Labels.AuthenticationRequired
            };

            this.Buttons = new List<CardAction> { button };
        }
    }
}