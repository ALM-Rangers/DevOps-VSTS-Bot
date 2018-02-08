// ———————————————————————————————
// <copyright file="GuardExtensions.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Provides extensions for Guards.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;

    /// <summary>
    /// Provides extensions for guards.
    /// </summary>
    public static class GuardExtensions
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if value is Guid.Empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        public static void ThrowIfEmpty(this Guid value, string paramName)
        {
            if (value == Guid.Empty)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if value is null.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        public static void ThrowIfNull(this object value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if value is null or a whitespace.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        public static void ThrowIfNullOrWhiteSpace(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(paramName);
            }
        }

        /// <summary>
        /// Throws an <see cref="ArgumentOutOfRangeException"/> if value is smaller or equal to 0.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="paramName">The name of the parameter.</param>
        public static void ThrowIfSmallerOrEqual(this int value, string paramName)
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName);
            }
        }
    }
}