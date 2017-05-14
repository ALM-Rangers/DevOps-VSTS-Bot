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
        [StepArgumentTransformation("'config:(.+)'")]
        public string TransformConfigProperty(string propertyName)
        {
            var type = typeof(Config);
            var property = type.GetProperty(propertyName);

            return property.GetValue(null, null).ToString();
        }

        [Given(@"I started a conversation as '(.*)'")]
        public void GivenIStartedAConversationAs(string userName)
        {
            var client = new DirectLineClient(Config.BotSecret);
            var conversation = client.Conversations.StartConversation();

            Config.Client = client;
            Config.ConversationId = conversation.ConversationId;
            Config.UserName = userName;
        }

        [Given(@"The user has previously logged in into the account and team project '(.*)'")]
        public void GivenTheUserHasPreviouslyLoggedInIntoTheAccountAndTeamProject(string teamProject)
        {
            var profile = new VstsProfile();
            profile.Accounts.Add(Config.Account);

            var userData = Config.BotState.GetUserData(ChannelIds.Directline, Config.UserName);
            userData.SetCurrentAccount(Config.Account);
            userData.SetCurrentProfile(profile);
            userData.SetProfiles(new List<VstsProfile> { profile });
            userData.SetCurrentTeamProject(teamProject);
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
            var matches = Regex.Matches(message, "'config:(.+)'");
            foreach (Match match in matches)
            {
                var value = this.TransformConfigProperty(match.Groups[1].Value);
                message = message.Replace(match.Groups[0].Value, value);
            }

            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.LastOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));

            activity.Text.ShouldBeEquivalentTo(message);
        }
    }
}
