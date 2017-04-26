// ———————————————————————————————
// <copyright file="BotDataBagExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains extensions for the IBotDataBag.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using Microsoft.Bot.Builder.Dialogs;

    /// <summary>
    /// Extensions for <see cref="IBotDataBag"/>.
    /// </summary>
    public static class BotDataBagExtensions
    {
        private const string Account = "Account";

        /// <summary>
        /// Gets the account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>A string representing the account.</returns>
        public static string GetAccount(this IBotDataBag dataBag)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.Get<string>(Account);
        }

        /// <summary>
        /// Sets the account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <param name="account">the account.</param>
        public static void SetAccount(this IBotDataBag dataBag, string account)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            dataBag.SetValue(account, Account);
        }
    }
}