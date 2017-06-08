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
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
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
            Config.BotState.SetUserData(ChannelIds.Directline, Config.UserName, new BotData());
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
        public async Task GivenIsAuthorized()
        {
            var authService = new AuthenticationService(Config.AppSecret, new Uri("https://authorize.url"));
            var vstsService = new VstsService();

            var token = await authService.GetToken(new OAuthToken { RefreshToken = Config.RefreshToken });
            var p = await vstsService.GetProfile(token);
            var accounts = await vstsService.GetAccounts(token, p.Id);
            var profile = new VstsProfile
            {
                Accounts = accounts.Select(a => a.AccountName).ToList(),
                Id = p.Id,
                EmailAddress = p.EmailAddress,
                Token = token
            };

            var data = await Config.BotState.GetUserDataAsync(ChannelIds.Directline, Config.UserName);

            data.SetProfile(profile);
            data.SetProfiles(new List<VstsProfile> { profile });

            await Config.BotState.SetUserDataAsync(ChannelIds.Directline, Config.UserName, data);

            Config.Profile = profile;
            Config.Token = token;
        }
    }
}
