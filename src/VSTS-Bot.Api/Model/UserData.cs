// ———————————————————————————————
// <copyright file="UserData.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the user data.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.VisualStudio.Services.Common;

    /// <summary>
    /// Represents the user data.
    /// </summary>
    [DataContract]
    public class UserData
    {
        /// <summary>
        /// Gets or sets the selected account.
        /// </summary>
        [DataMember]
        public string Account { get; set; }

        /// <summary>
        /// Gets or sets the Pin.
        /// </summary>
        [DataMember]
        public string Pin { get; set; }

        /// <summary>
        /// Gets the selected Profile.
        /// </summary>
        public Profile Profile
        {
            get { return this.Profiles.FirstOrDefault(p => p.IsSelected); }
        }

        /// <summary>
        /// Gets the not validated Profile.
        /// </summary>
        public Profile ProfileNotValidated
        {
            get { return this.Profiles.FirstOrDefault(p => !p.IsValidated); }
        }

        /// <summary>
        /// Gets or sets a list of profiles.
        /// </summary>
        [DataMember]
        public IList<Profile> Profiles { get; set; } = new List<Profile>();

        /// <summary>
        /// Gets or sets the selected team project.
        /// </summary>
        [DataMember]
        public string TeamProject { get; set; }

        /// <summary>
        /// Selects a profile.
        /// </summary>
        /// <param name="profile">The profile to select.</param>
        public void SelectProfile(Profile profile)
        {
            this.Profiles.ForEach(p => p.IsSelected = false);
            var selected = this.Profiles.FirstOrDefault(p => p.Id == profile.Id) ?? profile;

            selected.IsSelected = true;
            selected.IsValidated = true;

            if (this.Profiles.All(p => p.Id != selected.Id))
            {
                this.Profiles.Add(profile);
            }
        }
    }
}