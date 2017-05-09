// ———————————————————————————————
// <copyright file="EchoSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the specflow steps to perform an echo.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Temporary.
    /// </summary>
    [Binding]
    public sealed class EchoSteps
    {
        /// <summary>
        /// Temporary.
        /// </summary>
        /// <param name="message">Temp.</param>
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

        /// <summary>
        /// Temp.
        /// </summary>
        /// <param name="response">temp.</param>
        [Then(@"I should receive a response '(.*)'")]
        public void ThenIShouldReceiveAResponse(string response)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.LastOrDefault(a => string.CompareOrdinal(a.From.Id, Config.BotId) == 0);

            activity.Text.ShouldBeEquivalentTo(response);
        }
    }
}
