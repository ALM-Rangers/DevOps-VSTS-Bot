// ———————————————————————————————
// <copyright file="ConnectSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// $SUMMARY$
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
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
    public class ConnectSteps
    {
        [When(@"I connect to the account and '(.*)'")]
        public void WhenIConnectToTheAccountAnd(KeyValuePair<string, string> teamProject)
        {
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount(Config.UserName, Config.UserName),
                Text = FormattableString.Invariant($"connect {Config.Account} {teamProject.Value}")
            };

            Config.Client.Conversations.PostActivity(Config.ConversationId, activity);
        }

        [Then(@"I am connected to the account and '(.*)'")]
        public void ThenIAmConnectedToTheAccountAnd(KeyValuePair<string, string> teamProject)
        {
            var account = Config.BotData.UserData.GetValue<string>("Account");
            var project = Config.BotData.UserData.GetValue<string>("TeamProject");

            account.ToUpperInvariant().Should().Be(Config.Account.ToUpperInvariant());
            project.ToUpperInvariant().Should().Be(teamProject.Value.ToUpperInvariant());
        }
    }
}