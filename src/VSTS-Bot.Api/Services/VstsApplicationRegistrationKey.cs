// ———————————————————————————————
// <copyright file="VstsApplicationRegistrationKey.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents registration key.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Represents registration key that can be used in <see cref="IVstsApplicationRegistry.GetVstsApplicationRegistration"/>
    /// /// </summary>
    public class VstsApplicationRegistrationKey
    {
        private readonly string channelId;

        private readonly string userId;

        /// <summary>
        /// Initializes a new instance of the <see cref="VstsApplicationRegistrationKey"/> class using channel and user ID.
        /// </summary>
        /// <param name="channelId">Bot channel ID</param>
        /// <param name="userId">Bot user ID</param>
        public VstsApplicationRegistrationKey(string channelId, string userId)
        {
            this.channelId = channelId ?? throw new ArgumentNullException(nameof(channelId));
            this.userId = userId ?? throw new ArgumentNullException(nameof(userId));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VstsApplicationRegistrationKey"/> class using <see cref="IMessageActivity"/>.
        /// </summary>
        /// <param name="activity"><see cref="IMessageActivity"/> interface to get channel and user ID from.</param>
        public VstsApplicationRegistrationKey(IMessageActivity activity)
        {
            this.channelId = activity?.ChannelId ?? throw new ArgumentNullException(nameof(activity));
            this.userId = activity.From?.Id;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return FormattableString.Invariant($"{this.channelId}/{this.userId}");
        }
    }
}