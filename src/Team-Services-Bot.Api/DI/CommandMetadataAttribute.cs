// ———————————————————————————————
// <copyright file="CommandMetadataAttribute.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the meta data used to connect a command to a dialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot
{
    using System;
    using System.ComponentModel.Composition;

    /// <summary>
    /// Represents an <see cref="Attribute"/> for commands coming from the users.
    /// </summary>
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CommandMetadataAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandMetadataAttribute"/> class.
        /// </summary>
        /// <param name="command">The actual command.</param>
        public CommandMetadataAttribute(string command)
        {
            this.Command = command;
        }

        /// <summary>
        /// Gets the command coming from the users.
        /// </summary>
        public string Command { get; private set; }
    }
}