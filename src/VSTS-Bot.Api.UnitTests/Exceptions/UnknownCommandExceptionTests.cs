// ———————————————————————————————
// <copyright file="UnknownCommandExceptionTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for the unknown command exception.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Exceptions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class UnknownCommandExceptionTests
    {
        [TestMethod]
        public void Serialize_Exception()
        {
            var target = new UnknownCommandException();
            target = new UnknownCommandException("message", new Exception());
            target = new UnknownCommandException("Command1", "message", new Exception());

            var @string = target.ToString();

            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                // "Save" object state
                bf.Serialize(ms, target);

                // Re-use the same stream for de-serialization
                ms.Seek(0, 0);

                // Replace the original exception with de-serialized one
                target = (UnknownCommandException)bf.Deserialize(ms);
            }

            // Double-check that the exception message and stack trace (owned by the base Exception) are preserved
            target.ToString().Should().Be(@string);
        }
    }
}