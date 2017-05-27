// ———————————————————————————————
// <copyright file="BotDataBagExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides extensions for IBotDataBag and BotData.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Provides extensions for <see cref="IBotDataBag"/> and <see cref="BotData"/>.
    /// </summary>
    public static class BotDataBagExtensions
    {
        private const string Account = "Account";
        private const string Pin = "Pin";
        private const string Profile = "Profile";
        private const string Profiles = "Profiles";
        private const string TeamProject = "TeamProject";

        /// <summary>
        /// Gets the current account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>A <see cref="VstsAccount"/> representing the account.</returns>
        public static VstsAccount GetCurrentAccount(this IBotDataBag dataBag)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            VstsAccount account;

            return dataBag.TryGetValue(Account, out account) ? account : null;
        }

        /// <summary>
        /// Gets the current profile.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>A string representing the account.</returns>
        public static VstsProfile GetCurrentProfile(this IBotDataBag dataBag)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            VstsProfile profile;

            return dataBag.TryGetValue(Profile, out profile) ? profile : null;
        }

        /// <summary>
        /// Gets the current team project for the user.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>the name of the current team project.</returns>
        public static string GetCurrentTeamProject(this IBotDataBag dataBag)
        {
            string teamProject;
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.TryGetValue(TeamProject, out teamProject) ? teamProject : string.Empty;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <returns>A pin.</returns>
        public static string GetPin(this BotData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return data.GetProperty<string>(Pin) ?? string.Empty;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        /// <param name="dataBag">The dataBag.</param>
        /// <returns>A pin.</returns>
        public static string GetPin(this IBotDataBag dataBag)
        {
            string result;
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            return dataBag.TryGetValue(Pin, out result) ? result : string.Empty;
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
        /// <param name="data">Bot data</param>
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
        /// Sets the current account.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="account">The account.</param>
        public static void SetCurrentAccount(this BotData data, VstsAccount account)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            data.SetProperty(Account, account);
        }

        /// <summary>
        /// Sets the current account.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <param name="account">the account.</param>
        public static void SetCurrentAccount(this IBotDataBag dataBag, VstsAccount account)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (account == null)
            {
                throw new ArgumentNullException(nameof(account));
            }

            dataBag.SetValue(Account, account);
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
        /// Sets the current team project.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="teamProject">The team project.</param>
        public static void SetCurrentTeamProject(this BotData data, string teamProject)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            data.SetProperty(TeamProject, teamProject);
        }

        /// <summary>
        /// Sets the current team project.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="teamProject">The team project.</param>
        public static void SetCurrentTeamProject(this IBotDataBag dataBag, string teamProject)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (string.IsNullOrWhiteSpace(teamProject))
            {
                throw new ArgumentNullException(nameof(teamProject));
            }

            dataBag.SetValue(TeamProject, teamProject);
        }

        /// <summary>
        /// Sets the pin.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="pin">The pin.</param>
        public static void SetPin(this IBotDataBag dataBag, string pin)
        {
            if (dataBag == null)
            {
                throw new ArgumentNullException(nameof(dataBag));
            }

            if (string.IsNullOrWhiteSpace(pin))
            {
                throw new ArgumentNullException(nameof(pin));
            }

            dataBag.SetValue(Pin, pin);
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