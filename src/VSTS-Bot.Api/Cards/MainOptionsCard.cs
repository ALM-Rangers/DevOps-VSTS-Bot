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
        /// <param name="isConnected">Indicates that there is a connection to an account / team project.</param>
        public MainOptionsCard(bool isConnected)
        {
            if (isConnected)
            {
                this.Text = Labels.HelpAll;

                var button1 = new CardAction(ActionTypes.ImBack, Labels.Approvals, value: "approvals");
                var button2 = new CardAction(ActionTypes.ImBack, Labels.Builds, value: "builds");
                var button3 = new CardAction(ActionTypes.ImBack, Labels.Connect, value: "connect");
                var button4 = new CardAction(ActionTypes.ImBack, Labels.Releases, value: "releases");
                var button5 = new CardAction(ActionTypes.ImBack, Labels.Disconnect, value: "disconnect");
                var button6 = new CardAction(ActionTypes.ImBack, Labels.Subscriptions, value: "subscriptions");
                var button7 = new CardAction(ActionTypes.ImBack, Labels.Export, value: "export");
                this.Buttons = new List<CardAction> { button1, button2, button3, button4, button5, button6, button7 };
            }
            else
            {
                this.Text = Labels.HelpConnect;

                var button = new CardAction(ActionTypes.ImBack, Labels.Connect, value: "connect");
                this.Buttons = new List<CardAction> { button };
            }
        }
    }
}