// ———————————————————————————————
// <copyright file="ProjectsCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>

namespace Vsar.TSBot.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the card for a list of projects.
    /// </summary>
    public class ProjectsCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectsCard"/> class.
        /// </summary>
        /// <param name="projects">The account.</param>
        public ProjectsCard(IEnumerable<string> projects)
        {
            this.Buttons = projects
                .Select(p => new CardAction(ActionTypes.ImBack, p, value: p))
                .ToList();
        }
    }
}