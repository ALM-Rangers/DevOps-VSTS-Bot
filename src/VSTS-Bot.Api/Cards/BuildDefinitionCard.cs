// ———————————————————————————————
// <copyright file="BuildDefinitionCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a card for a build definition.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.Cards
{
    using System;
    using Microsoft.Bot.Connector;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Resources;

    /// <summary>
    /// Represents a card for a build definition.
    /// </summary>
    public class BuildDefinitionCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDefinitionCard"/> class.
        /// </summary>
        /// <param name="buildDefinition">A  build definition.</param>
        public BuildDefinitionCard(BuildDefinitionReference buildDefinition)
        {
            buildDefinition.ThrowIfNull(nameof(buildDefinition));

            this.Title = buildDefinition.Name;

            this.Buttons.Add(new CardAction(ActionTypes.ImBack, Labels.Queue, value: FormattableString.Invariant($"queue {buildDefinition.Id}")));
        }
    }
}