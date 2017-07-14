// ———————————————————————————————
// <copyright file="BuildSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the steps for builds.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using Newtonsoft.Json.Linq;
    using TechTalk.SpecFlow;

    [Binding]
    public class BuildSteps
    {
        [Given(@"I get a list of build definitions")]
        [Then(@"I get a list of build definitions")]
        public void ThenIGetAListOfBuildDefinitions(Table table)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.LastOrDefault(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));
            var cards = activity.Attachments.Select(a => a.Content).Cast<JObject>().ToList();

            cards.Should().HaveCount(table.RowCount);

            for (var index = 0; index < cards.Count; index++)
            {
                var row = table.Rows[index];
                var card = cards[index];

                card["title"].Value<string>().Should().Be(row["Name"]);
            }
        }

        [Then(@"A build with id '(.*)' should exist on '(.*)'")]
        public void ThenABuildWithIdShouldExist(int buildId, KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();

            var build = service.GetBuildAsync(Config.Account, teamProject.Value, buildId, Config.Token).Result;

            build.Should().NotBeNull();
        }
    }
}
