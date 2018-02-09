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
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
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
            var botData = Config.GetBotData();
            botData.LoadAsync(CancellationToken.None).Wait();

            botData.UserData.TryGetValue("userData", out UserData data);

            data.Account.ToUpperInvariant().Should().Be(Config.Account.ToUpperInvariant());
            data.TeamProject.ToUpperInvariant().Should().Be(teamProject.Value.ToUpperInvariant());
        }
    }
}