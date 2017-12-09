// ———————————————————————————————
// <copyright file="Subscription.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a subscription.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a subscription.
    /// </summary>
    [DataContract]
    [Serializable]
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the channel id.
        /// </summary>
        [DataMember]
        public string ChannelId { get; set; }

        /// <summary>
        /// Gets the id.
        /// </summary>
        [DataMember]
        [JsonProperty("id")]
        [Key]
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets a value indicating whether the Subscription has been activated.
        /// </summary>
        [DataMember]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [DataMember]
        public SubscriptionType SubscriptionType { get; set; }

        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
    }
}