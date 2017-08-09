// ———————————————————————————————
// <copyright file="AttachmentCollectionExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides extensions for a list with attachments.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Provides extensions for a list with <see cref="Attachment"/>.
    /// </summary>
    public static class AttachmentCollectionExtensions
    {
        /// <summary>
        /// Adds and converts a card to the attachments list.
        /// </summary>
        /// <param name="attachments">A list of attachments.</param>
        /// <param name="card">The card.</param>
        public static void Add(this IList<Attachment> attachments, HeroCard card)
        {
            attachments.ThrowIfNull(nameof(attachments));
            card.ThrowIfNull(nameof(card));

            attachments.Add(card.ToAttachment());
        }

        /// <summary>
        /// Adds and converts a card to the attachments list.
        /// </summary>
        /// <param name="attachments">A list of attachments.</param>
        /// <param name="card">The card.</param>
        public static void Add(this IList<Attachment> attachments, SigninCard card)
        {
            attachments.ThrowIfNull(nameof(attachments));
            card.ThrowIfNull(nameof(card));

            attachments.Add(card.ToAttachment());
        }
    }
}