// ———————————————————————————————
// <copyright file="IVstsApplicationRegistry.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an interface for retriving VSTS Application Registration based on session key.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    /// <summary>
    /// Represents an interface for retriving VSTS Application Registration based on session key
    /// </summary>
    public interface IVstsApplicationRegistry
    {
        /// <summary>
        /// Gets <see cref="VstsApplication"/> representing VSTS application for specified session key.
        /// </summary>
        /// <param name="sessionKey">Free format <see cref="string"/> representing session key.</param>
        /// <returns>Instance of the <see cref="VstsApplication"/> class.</returns>
        VstsApplication GetVstsApplicationRegistration(VstsApplicationRegistrationKey sessionKey);
    }
}
