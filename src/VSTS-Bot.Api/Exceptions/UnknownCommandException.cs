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
    using System.Globalization;
    using System.Runtime.Serialization;
    using Resources;

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
            : this(string.Empty, string.Empty, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="commandName">The name of the unknown command.</param>
        public UnknownCommandException(string commandName)
            : this(commandName, string.Format(CultureInfo.CurrentCulture, Exceptions.UnknownCommandException, commandName), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="commandName">A commandName.</param>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An <see cref="Exception"/> as inner exception.</param>
        public UnknownCommandException(string commandName, string message, Exception innerException)
            : base(message, innerException)
        {
            this.CommandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="message">A message.</param>
        /// <param name="innerException">An <see cref="Exception"/> as inner exception.</param>
        public UnknownCommandException(string message, Exception innerException)
            : this(string.Empty, message, innerException)
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
            this.CommandName = info.GetString(nameof(this.CommandName));
        }

        /// <summary>
        /// Gets the name of the unknown command
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string CommandName { get; } = string.Empty;

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.ThrowIfNull(nameof(info));

            info.AddValue(nameof(this.CommandName), this.CommandName);
            base.GetObjectData(info, context);
        }
    }
}