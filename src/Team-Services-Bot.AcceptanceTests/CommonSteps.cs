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

        [Given(@"The user has previously logged in into the account and team project '(.*)'")]
        public void GivenTheUserHasPreviouslyLoggedInIntoTheAccountAndTeamProject(KeyValuePair<string, string> pair)
        {
            var profile = new VstsProfile();
            profile.Accounts.Add(Config.Account);
            profile.Token = new OAuthToken { ExpiresIn = 3600 };

            var userData = Config.BotState.GetUserData(ChannelIds.Directline, Config.UserName);

            userData.SetCurrentAccount(Config.Account);
            userData.SetCurrentProfile(profile);
            userData.SetProfiles(new List<VstsProfile> { profile });
            userData.SetCurrentTeamProject(pair.Value);

            Config.BotState.SetUserData(ChannelIds.Directline, Config.UserName, userData);
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
    }
}
