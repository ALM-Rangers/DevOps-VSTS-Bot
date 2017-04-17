//———————————————————————————————
// <copyright file=”name of this file, i.e. CommandMetadataAttribute.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the meta data used to connect a command to a dialog.
// </summary>
//———————————————————————————————

using System;
using System.ComponentModel.Composition;

namespace Vsar.TSBot.DI
{
    [MetadataAttribute, AttributeUsage(AttributeTargets.Class)]
    public class CommandMetadataAttribute : Attribute
    {
        public CommandMetadataAttribute(string command)
        {
            Command = command;
        }

        public string Command { get; private set; }
    }
}