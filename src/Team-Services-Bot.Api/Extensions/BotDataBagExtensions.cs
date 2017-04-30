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
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Extensions for <see cref="IBotDataBag"/>.
    /// </summary>
    public static class BotDataBagExtensions
    {
        private const string Account = "Account";
        private const string Profile = "VSTSProfile";

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

        /// <summary>
        /// Gets the profile.
        /// </summary>
        /// <param name="data">the Botdata</param>
        /// <returns>The profile.</returns>
        public static VstsProfile GetProfile(this BotData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return data.GetProperty<VstsProfile>(Profile);
        }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="data">the botdats.</param>
        /// <param name="profile">the profile.</param>
        public static void SetProfile(this BotData data, VstsProfile profile)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            data.SetProperty(Profile, profile);
        }
    }
}