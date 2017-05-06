// ———————————————————————————————
// <copyright file="Authorize.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents a model for the authorize view.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    /// <summary>
    /// Represents the Authorize model.
    /// </summary>
    public class Authorize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Authorize"/> class.
        /// </summary>
        /// <param name="pin">The pin</param>
        public Authorize(string pin)
        {
            this.Pin = pin;
        }

        /// <summary>
        /// Gets the pin.
        /// </summary>
        public string Pin { get; }
    }
}