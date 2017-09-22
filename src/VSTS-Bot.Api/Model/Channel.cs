// ———————————————————————————————
// <copyright file="Channel.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the supported channels.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Represents a channel.
    /// </summary>
    [Flags]
    public enum Channel
    {
        /// <summary>
        /// Skype channel.
        /// </summary>
        Skype = 1,

        /// <summary>
        /// Slack channel.
        /// </summary>
        Slack = 2,

        /// <summary>
        /// Teams channel.
        /// </summary>
        Teams = 4,

        /// <summary>
        /// All the channels.
        /// </summary>
        All = 7
    }
}