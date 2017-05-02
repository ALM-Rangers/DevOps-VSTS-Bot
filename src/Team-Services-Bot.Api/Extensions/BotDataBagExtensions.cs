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
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Extensions for <see cref="IBotDataBag"/>.
    /// </summary>
    public static class BotDataBagExtensions
    {
        private const string Account = "Account";
        private const string Profile = "Profile";
        private const string Profiles = "Profiles";

        /// <summary>
        /// Gets the current account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>A string representing the account.</returns>
        public static string GetCurrentAccount(this IBotDataBag dataBag)
        {
            string result;

            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.TryGetValue(Account, out result) ? result : string.Empty;
        }

        /// <summary>
        /// Get the current profile.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <returns>A VstsProfile.</returns>
        public static VstsProfile GetProfile(this IBotDataBag dataBag)
        {
            VstsProfile profile;

            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.TryGetValue(Profile, out profile) ? profile : null;
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <param name="data">the Botdata</param>
        /// <returns>The profile.</returns>
        public static IList<VstsProfile> GetProfiles(this BotData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return data.GetProperty<IList<VstsProfile>>(Profiles) ?? new List<VstsProfile>();
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <param name="dataBag">The data.</param>
        /// <returns>A list of profiles.</returns>
        public static IList<VstsProfile> GetProfiles(this IBotDataBag dataBag)
        {
            IList<VstsProfile> results;

            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.TryGetValue(Profiles, out results) ? results : new List<VstsProfile>();
        }

        /// <summary>
        /// Sets the current account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <param name="account">the account.</param>
        public static void SetCurrentAccount(this IBotDataBag dataBag, string account)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (string.IsNullOrWhiteSpace(account))
            {
                throw new ArgumentNullException(nameof(account));
            }

            dataBag.SetValue(account, Account);
        }

        /// <summary>
        /// Sets the current vsts profile.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="profile">The profile.</param>
        public static void SetCurrentProfile(this BotData data, VstsProfile profile)
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

        /// <summary>
        /// Sets the current vsts profile.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="profile">The profile.</param>
        public static void SetCurrentProfile(this IBotDataBag dataBag, VstsProfile profile)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            dataBag.SetValue(Profile, profile);
        }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="data">the botdats.</param>
        /// <param name="profiles">the profile.</param>
        public static void SetProfiles(this BotData data, IList<VstsProfile> profiles)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            data.SetProperty(Profiles, profiles);
        }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="dataBag">the botdats.</param>
        /// <param name="profiles">the profile.</param>
        public static void SetProfiles(this IBotDataBag dataBag, IList<VstsProfile> profiles)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (profiles == null)
            {
                throw new ArgumentNullException(nameof(profiles));
            }

            dataBag.SetValue(Profiles, profiles);
        }
    }
}