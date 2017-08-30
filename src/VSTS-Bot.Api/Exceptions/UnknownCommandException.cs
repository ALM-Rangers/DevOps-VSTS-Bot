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
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="commandName">The name of the unknown command.</param>
        public UnknownCommandException(string commandName)
            : base(string.Format(CultureInfo.CurrentCulture, Exceptions.UnknownCommandException, commandName))
        {
            this.CommandName = commandName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownCommandException"/> class.
        /// </summary>
        /// <param name="commandName">A commandName.</param>
        /// <param name="innerException">An <see cref="Exception"/> as inner exception.</param>
        public UnknownCommandException(string commandName, Exception innerException)
            : base(commandName, innerException)
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
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(this.CommandName), this.CommandName);
            base.GetObjectData(info, context);
        }
    }
}