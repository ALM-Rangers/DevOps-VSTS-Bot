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
            if (buildDefinition == null)
            {
                throw new ArgumentNullException(nameof(buildDefinition));
            }

            this.Title = buildDefinition.Name;
        }
    }
}