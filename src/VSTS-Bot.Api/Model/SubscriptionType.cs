// ———————————————————————————————
// <copyright file="SubscriptionType.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Indicates the subscription type.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates the subscription type.
    /// </summary>
    [DataContract]
    [Serializable]
    public enum SubscriptionType
    {
        /// <summary>
        /// The approvals of the current user.
        /// </summary>
        [EnumMember]
        MyApprovals
    }
}