// ———————————————————————————————
// <copyright file="MainOptionsCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the card that shows the main options.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Cards
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;
    using Resources;

    /// <summary>
    /// Represents the card that shows the main options.
    /// </summary>
    public class MainOptionsCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainOptionsCard"/> class.
        /// </summary>
        public MainOptionsCard()
        {
            var button1 = new CardAction(ActionTypes.ImBack, Labels.Approvals, value: "approvals");
            var button2 = new CardAction(ActionTypes.ImBack, Labels.QueueBuild, value: "build queue");
            var button3 = new CardAction(ActionTypes.ImBack, Labels.QueueRelease, value: "release queue");

            this.Buttons = new List<CardAction> { button1, button2, button3 };
        }
    }
}