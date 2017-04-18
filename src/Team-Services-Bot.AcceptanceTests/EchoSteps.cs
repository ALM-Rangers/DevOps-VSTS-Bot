//———————————————————————————————
// <copyright file=”name of this file, i.e. EchoSteps.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the specflow steps to perform an echo.
// </summary>
//———————————————————————————————

using System.Linq;
using FluentAssertions;
using Microsoft.Bot.Connector.DirectLine;
using TechTalk.SpecFlow;

namespace Vsar.TSBot.AcceptanceTests
{
    [Binding]
    public sealed class EchoSteps
    {
        [When(@"I send a message '(.*)'")]
        public void WhenISendAMessage(string message)
        {
            var activity = new Activity
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount("TestUser"),
                Text = message
            };

            Config.Client.Conversations.PostActivity(Config.ConversationId, activity);
        }

        [Then(@"I should receive a response '(.*)'")]
        public void ThenIShouldReceiveAResponse(string response)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.LastOrDefault(a => a.From.Id == Config.BotId);

            activity.Text.ShouldBeEquivalentTo(response);
        }
    }
}
