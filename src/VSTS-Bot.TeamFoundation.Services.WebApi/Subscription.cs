// ———————————————————————————————
// <copyright file="Subscription.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a VSTS Service Hook subscription.
// </summary>
// ———————————————————————————————

namespace VSTS_Bot.TeamFoundation.Services.WebApi
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.VisualStudio.Services.WebApi;

    /// <summary>
    ///     Represents a VSTS Service Hook subscription.
    /// </summary>
    [DataContract]
    public class Subscription
    {
        /// <summary>
        /// Gets or sets the action description.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ActionDescription { get; set; }

        /// <summary>
        /// Gets or sets the consumer action id.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ConsumerActionId { get; set; }

        /// <summary>
        /// Gets or sets the consumer id.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string ConsumerId { get; set; }

        /// <summary>
        /// Gets or sets the consumer inputs.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IDictionary<string, string> ConsumerInputs { get; set; }

        /// <summary>
        /// Gets or sets the identity that created the subscription.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IdentityRef CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets the date the subscription was created.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the event description.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string EventDescription { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string EventType { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identity that modified the subscription.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IdentityRef ModifiedBy { get; set; }

        /// <summary>
        /// Gets or sets the date the subscription was modified.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// Gets or sets the publisher id.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string PublisherId { get; set; }

        /// <summary>
        /// Gets or sets the publisher inputs.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public IDictionary<string, string> PublisherInputs { get; set; }

        /// <summary>
        /// Gets or sets the resource version.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public object ResourceVersion { get; set; }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        [DataMember(EmitDefaultValue = false)]
        public string Url { get; set; }
    }
}