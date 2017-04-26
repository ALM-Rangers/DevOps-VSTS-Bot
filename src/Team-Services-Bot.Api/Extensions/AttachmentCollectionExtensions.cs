// ———————————————————————————————
// <copyright file="AttachmentCollectionExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains extensions for IList{Attachment}.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Connector;

    /// <summary>
    /// Contains extensions for <see cref="IList{T}"/>
    /// </summary>
    public static class AttachmentCollectionExtensions
    {
        /// <summary>
        /// Adds and converts a card to the attachments list.
        /// </summary>
        /// <param name="attachments">A list of attachments.</param>
        /// <param name="card">The card.</param>
        public static void Add(this IList<Attachment> attachments, SigninCard card)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            attachments.Add(card.ToAttachment());
        }
    }
}