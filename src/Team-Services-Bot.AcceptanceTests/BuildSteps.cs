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
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
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

        [Then(@"I get a queued build response")]
        public void ThenIGetAQueuedBuildResponse()
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.Last(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));

            var match = Regex.Match(activity.Text, @"Build with id (\d+) is queued.");

            match.Success.Should().BeTrue();

            Config.BuildId = Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        [Then(@"A queued build should exist on '(.*)'")]
        public void ThenABuildWithIdShouldExist(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();

            var build = service.GetBuildAsync(Config.Account, teamProject.Value, Config.BuildId, Config.Token).Result;

            build.Should().NotBeNull();
        }
    }
}
