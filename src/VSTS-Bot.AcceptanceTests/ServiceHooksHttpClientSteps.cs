// ———————————————————————————————
// <copyright file="ServiceHooksHttpClientSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the Steps Service Hooks Http Client Steps.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using TechTalk.SpecFlow;

    [Binding]
    public sealed class ServiceHooksHttpClientSteps
    {
        [Given(@"I Created a Service Hook for '(.*)'")]
        public async Task WhenICreatedAServiceHook(KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();

            var teamProjects = await service.GetProjects(Config.Account, Config.Token);
            var tp = teamProjects.FirstOrDefault(p => p.Name.Equals(teamProject.Value, StringComparison.OrdinalIgnoreCase));

            var subscription = new VSTS_Bot.TeamFoundation.Services.WebApi.Subscription
            {
                 ConsumerActionId = "httpRequest",
                 ConsumerId = "webHooks",
                 ConsumerInputs = new Dictionary<string, string> { { "url", "https://myservice/myhookeventreceiver" } },
                 EventType = "build.complete",
                 PublisherId = "tfs",
                 PublisherInputs = new Dictionary<string, string> { { "buildStatus", "Failed" }, { "definitionName", "Build 1" }, { "projectId", tp.Id.ToString() } },
                 ResourceVersion = "1.0-preview.1"
            };

            subscription = await service.CreateSubscription(Config.Account, subscription, Config.Token);

            ScenarioContext.Current["SubscriptionId"] = subscription.Id;
        }

        [Then(@"I list the Service Hook")]
        public async Task ThenIListTheServiceHook()
        {
            var service = new VstsService();

            var subscriptions = await service.GetSubscriptions(Config.Account, Config.Token);

            subscriptions.Should().HaveCount(1);
        }

        [Then(@"I get the Service Hook")]
        public async Task ThenIGetTheServiceHook()
        {
            var subscriptionId = (Guid)ScenarioContext.Current["SubscriptionId"];
            var service = new VstsService();

            var subscription = await service.GetSubscription(Config.Account, subscriptionId, Config.Token);

            subscription.Should().NotBeNull();
        }

        [Then(@"I delete the Service Hook")]
        public async Task ThenIDeleteTheServiceHook()
        {
            var subscriptionId = (Guid)ScenarioContext.Current["SubscriptionId"];
            var service = new VstsService();

            await service.DeleteSubscription(Config.Account, subscriptionId, Config.Token);
        }
    }
}
