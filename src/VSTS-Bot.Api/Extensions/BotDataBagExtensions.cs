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
        private const string NotValidatedByPinProfile = "NotValidatedByPinProfile";
        private const string Pin = "Pin";
        private const string Profile = "Profile";
        private const string Profiles = "Profiles";
        private const string TeamProject = "TeamProject";

        /// <summary>
        /// Clears the NotValidatedByPinProfile.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        public static void ClearNotValidatedByPinProfile(this IBotDataBag dataBag)
        {
            dataBag.RemoveValue(NotValidatedByPinProfile);
        }

        /// <summary>
        /// Gets the current account name.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>An account.</returns>
        public static string GetAccount(this IBotDataBag dataBag)
        {
            dataBag.ThrowIfNull(nameof(dataBag));

            return dataBag.TryGetValue(Account, out string account) ? account : string.Empty;
        }

        /// <summary>
        /// Gets the current team project for the user.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>the name of the current team project.</returns>
        public static string GetTeamProject(this IBotDataBag dataBag)
        {
            dataBag.ThrowIfNull(nameof(dataBag));

            return dataBag.TryGetValue(TeamProject, out string teamProject) ? teamProject : string.Empty;
        }

        /// <summary>
        /// Gets the Not Validate By Pin Profile.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <returns>The <see cref="VstsProfile"/>.</returns>
        public static VstsProfile GetNotValidatedByPinProfile(this IBotDataBag dataBag)
        {
            dataBag.ThrowIfNull(nameof(dataBag));

            return dataBag.TryGetValue(NotValidatedByPinProfile, out VstsProfile profile) ? profile : null;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <returns>A pin.</returns>
        public static string GetPin(this BotData data)
        {
            data.ThrowIfNull(nameof(data));

            return data.GetProperty<string>(Pin) ?? string.Empty;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        /// <param name="dataBag">The dataBag.</param>
        /// <returns>A pin.</returns>
        public static string GetPin(this IBotDataBag dataBag)
        {
            dataBag.ThrowIfNull(nameof(dataBag));

            return dataBag.TryGetValue(Pin, out string result) ? result : string.Empty;
        }

        /// <summary>
        /// Get the current profile.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="authenticationService"><see cref="IAuthenticationService"/> object.</param>
        /// <returns>A VstsProfile.</returns>
        public static VstsProfile GetProfile(this IBotDataBag dataBag, IAuthenticationService authenticationService)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            authenticationService.ThrowIfNull(nameof(authenticationService));

            if (!dataBag.TryGetValue(Profile, out VstsProfile profile))
            {
                return null;
            }

            if (profile.Token.ExpiresOn.AddMinutes(-5) > DateTime.UtcNow)
            {
                return profile;
            }

            profile.Token = authenticationService.GetToken(profile.Token).Result;
            dataBag.SetProfile(profile);

            return profile;
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <param name="data">Bot data</param>
        /// <returns>The profile.</returns>
        public static IList<VstsProfile> GetProfiles(this BotData data)
        {
            data.ThrowIfNull(nameof(data));

            return data.GetProperty<IList<VstsProfile>>(Profiles) ?? new List<VstsProfile>();
        }

        /// <summary>
        /// Gets the profiles.
        /// </summary>
        /// <param name="dataBag">The data.</param>
        /// <returns>A list of profiles.</returns>
        public static IList<VstsProfile> GetProfiles(this IBotDataBag dataBag)
        {
            dataBag.ThrowIfNull(nameof(dataBag));

            return dataBag.TryGetValue(Profiles, out IList<VstsProfile> results) ? results : new List<VstsProfile>();
        }

        /// <summary>
        /// Sets the current account.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="account">The account.</param>
        public static void SetAccount(this BotData data, string account)
        {
            data.ThrowIfNull(nameof(data));
            account.ThrowIfNullOrWhiteSpace(nameof(account));

            data.SetProperty(Account, account);
        }

        /// <summary>
        /// Sets the current account.
        /// </summary>
        /// <param name="dataBag">The <see cref="IBotDataBag"/>.</param>
        /// <param name="account">the account.</param>
        public static void SetAccount(this IBotDataBag dataBag, string account)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            account.ThrowIfNullOrWhiteSpace(nameof(account));

            dataBag.SetValue(Account, account);
        }

        /// <summary>
        /// Sets the not validated by pin profile.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="profile">The profile.</param>
        public static void SetNotValidatedByPinProfile(this BotData data, VstsProfile profile)
        {
            data.ThrowIfNull(nameof(data));
            profile.ThrowIfNull(nameof(profile));

            data.SetProperty(NotValidatedByPinProfile, profile);
        }

        /// <summary>
        /// Sets the current vsts profile.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="profile">The profile.</param>
        public static void SetProfile(this BotData data, VstsProfile profile)
        {
            data.ThrowIfNull(nameof(data));
            profile.ThrowIfNull(nameof(profile));

            data.SetProperty(Profile, profile);
        }

        /// <summary>
        /// Sets the current vsts profile.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="profile">The profile.</param>
        public static void SetProfile(this IBotDataBag dataBag, VstsProfile profile)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            profile.ThrowIfNull(nameof(profile));

            dataBag.SetValue(Profile, profile);
        }

        /// <summary>
        /// Sets the current team project.
        /// </summary>
        /// <param name="data">The bot data.</param>
        /// <param name="teamProject">The team project.</param>
        public static void SetTeamProject(this BotData data, string teamProject)
        {
            data.ThrowIfNull(nameof(data));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));

            data.SetProperty(TeamProject, teamProject);
        }

        /// <summary>
        /// Sets the current team project.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="teamProject">The team project.</param>
        public static void SetTeamProject(this IBotDataBag dataBag, string teamProject)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            teamProject.ThrowIfNullOrWhiteSpace(nameof(teamProject));

            dataBag.SetValue(TeamProject, teamProject);
        }

        /// <summary>
        /// Sets the pin.
        /// </summary>
        /// <param name="dataBag">The data bag.</param>
        /// <param name="pin">The pin.</param>
        public static void SetPin(this IBotDataBag dataBag, string pin)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            pin.ThrowIfNullOrWhiteSpace(nameof(pin));

            dataBag.SetValue(Pin, pin);
        }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="data">the botdats.</param>
        /// <param name="profiles">the profile.</param>
        public static void SetProfiles(this BotData data, IList<VstsProfile> profiles)
        {
            data.ThrowIfNull(nameof(data));
            profiles.ThrowIfNull(nameof(profiles));

            data.SetProperty(Profiles, profiles);
        }

        /// <summary>
        /// Sets the profile.
        /// </summary>
        /// <param name="dataBag">the botdats.</param>
        /// <param name="profiles">the profile.</param>
        public static void SetProfiles(this IBotDataBag dataBag, IList<VstsProfile> profiles)
        {
            dataBag.ThrowIfNull(nameof(dataBag));
            profiles.ThrowIfNull(nameof(profiles));

            dataBag.SetValue(Profiles, profiles);
        }
    }
}