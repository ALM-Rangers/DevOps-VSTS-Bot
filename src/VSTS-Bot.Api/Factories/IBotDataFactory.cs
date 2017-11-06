// ———————————————————————————————
// <copyright file="IBotDataFactory.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Interface for instantiating an IBotData.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;

    /// <summary>
    /// Interface for instantiatiing an <see cref="IBotData"/>;
    /// </summary>
    public interface IBotDataFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="IBotData"/>
        /// </summary>
        /// <param name="address">The <see cref="Address"/>.</param>
        /// <returns>A <see cref="IBotData"/>.</returns>
        IBotData Create(Address address);
    }
}