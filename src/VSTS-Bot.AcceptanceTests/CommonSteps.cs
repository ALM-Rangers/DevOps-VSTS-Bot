// ———————————————————————————————
// <copyright file="CommonSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides common steps that can be reused over features.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using TechTalk.SpecFlow;
    using Activity = Microsoft.Bot.Connector.DirectLine.Activity;
    using ActivityTypes = Microsoft.Bot.Connector.DirectLine.ActivityTypes;
    using ChannelAccount = Microsoft.Bot.Connector.DirectLine.ChannelAccount;

    [Binding]
    public sealed class CommonSteps
    {
        [Given(@"A user '(.*)'")]
        public void GivenAUser(string userName)
        {
            Config.UserName = userName;
        }

        [Given(@"A clean state")]
        public void GivenACleanState()
        {
            var botData = Config.GetBotData();
            botData.LoadAsync(CancellationToken.None).Wait();

            // Keep the profile for the refresh token.
            if (botData.UserData.TryGetValue("userData", out UserData data))
            {
                data.Account = string.Empty;
                data.TeamProject = string.Empty;

                botData.UserData.SetValue("userData", data);
            }

            botData.FlushAsync(CancellationToken.None).Wait();
        }

        [StepArgumentTransformation("config:(.+)")]
        public KeyValuePair<string, string> TransformConfigProperty(string propertyName)
        {
            var type = typeof(Config);
            var property = type.GetProperty(propertyName);

            var value = property.GetValue(null, null).ToString();

            return new KeyValuePair<string, string>(propertyName, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "As we are using Specflow we can not determine when the client is out of scope.")]
        [Given(@"I started a conversation")]
        public void GivenIStartedAConversation()
        {
            var client = new DirectLineClient(Config.BotSecret);
            var conversation = client.Conversations.StartConversation();

            Config.Client = client;
            Config.ConversationId = conversation.ConversationId;
        }

        [Given(@"I say '(.*)'")]
        [When(@"I say '(.*)'")]
        public void WhenISay(string message)
        {
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount(Config.UserName, Config.UserName),
                Text = message
            };

            Config.Client.Conversations.PostActivity(Config.ConversationId, activity);
        }

        [Then(@"the bot should respond with '(.*)'")]
        public void ThenTheBotShouldRespondWith(string message)
        {
            var matches = Regex.Matches(message, "'config:(.+?)'");
            foreach (Match match in matches)
            {
                var pair = this.TransformConfigProperty(match.Groups[1].Value);
                message = message.Replace(match.Groups[0].Value, FormattableString.Invariant($"'{pair.Value}'"));
            }

            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.FirstOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));

            activity.Text.ShouldBeEquivalentTo(message);
        }

        [Given(@"Is authorized")]
        public void GivenIsAuthorized()
        {
            var authService = new AuthenticationService();
            var vstsService = new VstsService();

            var botData = Config.GetBotData();
            botData.LoadAsync(CancellationToken.None).Wait();

            if (!botData.UserData.TryGetValue("userData", out UserData data))
            {
                data = new UserData();
            }

            var refreshToken = Config.RefreshToken;

            if (data.Profile != null && !Config.RefreshTokenReinitialize)
            {
                refreshToken = data.Profile.Token.RefreshToken;
            }

            Config.RefreshTokenReinitialize = false;

            var oldToken = new OAuthToken
            {
                AppSecret = Config.AppSecret,
                AuthorizeUrl = Config.AuthorizeUrl,
                RefreshToken = refreshToken
            };
            var token = authService.GetToken(oldToken).Result;
            var p = vstsService.GetProfile(token).Result;
            var accounts = vstsService.GetAccounts(token, p.Id).Result;
            var profile = new Profile
            {
                Accounts = accounts.Select(a => a.AccountName).ToList(),
                Id = p.Id,
                Token = token
            };

            data.Profiles.Clear();
            data.SelectProfile(profile);

            botData.UserData.SetValue("userData", data);

            botData.FlushAsync(CancellationToken.None).Wait();

            Config.Profile = profile;
            Config.Token = token;
        }

        [Given(@"I am connected to '(.*)'")]
        public void GivenIAmConnectedTo(KeyValuePair<string, string> teamProject)
        {
            var botData = Config.GetBotData();
            botData.LoadAsync(CancellationToken.None).Wait();

            if (!botData.UserData.TryGetValue("userData", out UserData data))
            {
                data = new UserData();
            }

            data.Account = Config.Account;
            data.TeamProject = teamProject.Value;

            botData.UserData.SetValue("userData", data);

            botData.FlushAsync(CancellationToken.None);
        }

        [Then(@"the bot should respond with the welcome message\.")]
        public void ThenTheBotShouldRespondWithTheWelcomeMessage_()
        {
            var pattern = "Hi Test User. Good to see you. I will help you with your Visual Studio Team Services tasks. Please read the \\[MIT License\\]\\(.+\\) if you have not done so.";

            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.FirstOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));

            activity.Text.Should().MatchRegex(pattern);
        }
    }
}
