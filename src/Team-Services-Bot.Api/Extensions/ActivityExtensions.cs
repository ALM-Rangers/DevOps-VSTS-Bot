// ———————————————————————————————
// <copyright file="ActivityExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides extensions for IActivity.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Provides extensions for <see cref="IActivity"/>.
    /// </summary>
    public static class ActivityExtensions
    {
        private const string Teams = "msteams";

        /// <summary>
        /// Checks if the <see cref="IActivity"/> comes from the MS Teams channel.
        /// </summary>
        /// <param name="activity">An <see cref="IActivity"/>.</param>
        /// <returns>A boolean value that indicates wether the activity comes from the MS Teams channel.</returns>
        public static bool IsTeamsChannel(this IActivity activity)
        {
            if (activity == null)
            {
                throw new ArgumentNullException(nameof(activity));
            }

            return activity.ChannelId.Equals(Teams, StringComparison.OrdinalIgnoreCase);
        }
    }
}