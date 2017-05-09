// ———————————————————————————————
// <copyright file="AccountCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the account from wich the user can choose.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Cards
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the Account Card.
    /// </summary>
    public class AccountCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountCard"/> class.
        /// </summary>
        /// <param name="account">The account.</param>
        public AccountCard(string account)
        {
            var button = new CardAction(ActionTypes.ImBack, account, value: FormattableString.Invariant($"connect {account}"));
            this.Buttons = new List<CardAction> { button };
        }
    }
}