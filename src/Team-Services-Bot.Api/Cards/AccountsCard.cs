// ———————————————————————————————
// <copyright file="AccountsCard.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the card for a list of accounts.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents the card for a list of accounts.
    /// </summary>
    public class AccountsCard : HeroCard
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AccountsCard"/> class.
        /// </summary>
        /// <param name="accounts">The account.</param>
        public AccountsCard(IEnumerable<string> accounts)
        {
            this.Buttons = accounts
                .Select(a => new CardAction(ActionTypes.ImBack, a, value: FormattableString.Invariant($"connect {a}")))
                .ToList();
        }
    }
}