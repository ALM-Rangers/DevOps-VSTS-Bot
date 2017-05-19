// ———————————————————————————————
// <copyright file="ApprovalCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a card for an approval.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Cards
{
    using System;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Resources;

    /// <summary>
    /// Represents a card for an approval.
    /// </summary>
    public class ApprovalCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalCard"/> class.
        /// </summary>
        /// <param name="approval">A <see cref="ReleaseApproval"/>.</param>
        public ApprovalCard(ReleaseApproval approval)
        {
            if (approval == null)
            {
                throw new ArgumentNullException(nameof(approval));
            }

            this.Subtitle = approval.ReleaseReference.Name;
            this.Text = approval.ReleaseEnvironmentReference.Name;
            this.Title = approval.ReleaseDefinitionReference.Name;

            this.Tap = new CardAction(ActionTypes.OpenUrl, value: approval.ReleaseReference.Url);

            this.Buttons.Add(new CardAction(ActionTypes.ImBack, Labels.Approve, value: FormattableString.Invariant($"approve {approval.Id}")));
            this.Buttons.Add(new CardAction(ActionTypes.ImBack, Labels.Reject, value: FormattableString.Invariant($"reject {approval.Id}")));
        }
    }
}