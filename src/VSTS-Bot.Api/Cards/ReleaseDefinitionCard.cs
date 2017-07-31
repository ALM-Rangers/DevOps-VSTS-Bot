// ———————————————————————————————
// <copyright file="ReleaseDefinitionCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a card for a release definition.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Cards
{
    using System;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Resources;

    /// <summary>
    /// Represents a card for a release definition.
    /// </summary>
    public class ReleaseDefinitionCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReleaseDefinitionCard"/> class.
        /// </summary>
        /// <param name="releaseDefinition">A release definition.</param>
        public ReleaseDefinitionCard(ReleaseDefinition releaseDefinition)
        {
            releaseDefinition.ThrowIfNull(nameof(releaseDefinition));

            this.Title = releaseDefinition.Name;

            this.Buttons.Add(new CardAction(ActionTypes.ImBack, Labels.Create, value: FormattableString.Invariant($"create {releaseDefinition.Id}")));
        }
    }
}