// ———————————————————————————————
// <copyright file="UnknownCommandException.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents an exception for unknown commands.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an exception for unknown commands.
    /// </summary>
    [Serializable]
    public class UnknownCommandException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        public UnknownCommandException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="message">A message.</param>
        public UnknownCommandException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An <see cref="Exception"/> as inner exception.</param>
        public UnknownCommandException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/>.</param>
        /// <param name="context">The <see cref="StreamingContext"/>.</param>
        protected UnknownCommandException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}