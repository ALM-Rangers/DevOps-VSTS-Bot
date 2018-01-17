// ———————————————————————————————
// <copyright file="MyApprovalSubscriptionStrategy.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Subscription strategy for my approvals.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Strategies.Subscription
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using Microsoft.TeamFoundation.Core.WebApi;
    using VSTS_Bot.TeamFoundation.Services.WebApi;

    /// <summary>
    /// Subscription strategy for my approvals.
    /// </summary>
    public class MyApprovalSubscriptionStrategy : ISubscriptionStrategy
    {
        /// <inheritdoc />
        public bool CanGetSubscription(SubscriptionType subscriptionType)
        {
            return subscriptionType == SubscriptionType.MyApprovals;
        }

        /// <inheritdoc />
        public Subscription GetSubscription(Guid subscriptionId, TeamProjectReference teamProject)
        {
            var url = HttpContext.Current != null
                ? $"{HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)}/api/event"
                : string.Empty;

            return new Subscription
            {
                ConsumerActionId = "httpRequest",
                ConsumerId = "webHooks",
                ConsumerInputs = new Dictionary<string, string>
                {
                    { "url", url },
                    { "httpHeaders", $"subscriptionToken:{subscriptionId}" }
                },
                EventType = "ms.vss-release.deployment-approval-pending-event",
                PublisherId = "rm",
                PublisherInputs = new Dictionary<string, string>
                {
                    { "projectId", teamProject.Id.ToString() }
                },
                ResourceVersion = "3.0-preview.1"
            };
        }
    }
}