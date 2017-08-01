// ———————————————————————————————
// <copyright file="ReleaseSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the Steps for a release.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using FluentAssertions;
    using Microsoft.Bot.Connector.DirectLine;
    using Newtonsoft.Json.Linq;
    using TechTalk.SpecFlow;

    [Binding]
    public class ReleaseSteps
    {
        [Given(@"I started release '(\d*)' on '(.*)'")]
        public void GivenIStartedOn(int definitionId, KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();
            service.CreateReleaseAsync(Config.Account, teamProject.Value, definitionId, Config.Token).Wait();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }

        [Then(@"I get a list of release definitions")]
        public void ThenIGetAListOfReleaseDefinitions(Table table)
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.Last(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));
            var cards = activity.Attachments.Select(a => a.Content).Cast<JObject>().ToList();

            cards.Should().HaveCount(table.RowCount);

            for (var index = 0; index < cards.Count; index++)
            {
                var row = table.Rows[index];
                var card = cards[index];

                card["title"].Value<string>().Should().Be(row["Name"]);
            }
        }

        [Then(@"I get a created release response")]
        public void ThenIGetACreatedReleaseResponse()
        {
            var activities = Config.Client.Conversations.GetActivities(Config.ConversationId);
            var activity = activities.Activities.Last(a => string.Equals(a.From.Id, Config.BotId, StringComparison.OrdinalIgnoreCase));

            var match = Regex.Match(activity.Text, @"Release with id (\d+) is created.");

            match.Success.Should().BeTrue();

            Config.ReleaseId = Convert.ToInt32(match.Groups[1].Value, CultureInfo.InvariantCulture);
        }

        [Then(@"A created release should exist on '(.*)'")]
        public void ThenACreatedReleaseShouldExistOn(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();

            var build = service.GetReleaseAsync(Config.Account, teamProject.Value, Config.ReleaseId, Config.Token).Result;

            build.Should().NotBeNull();
        }
    }
}
