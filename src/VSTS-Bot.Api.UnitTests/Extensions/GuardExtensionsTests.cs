// ———————————————————————————————
// <copyright file="GuardExtensionsTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the Guard Extensions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class GuardExtensionsTests
    {
        [TestMethod]
        public void ThrowIfNull_Null_Object()
        {
            object target = null;

            Assert.ThrowsException<ArgumentNullException>(() => target.ThrowIfNull(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfNull_NotNull_Object()
        {
            object target = "A string";

            target.ThrowIfNull(nameof(target));
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_Null()
        {
            string target = null;

            Assert.ThrowsException<ArgumentNullException>(() => target.ThrowIfNullOrWhiteSpace(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_Empty_String()
        {
            var target = string.Empty;

            Assert.ThrowsException<ArgumentNullException>(() => target.ThrowIfNullOrWhiteSpace(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_WhiteSpace()
        {
            var target = " ";

            Assert.ThrowsException<ArgumentNullException>(() => target.ThrowIfNullOrWhiteSpace(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfNullOrWhiteSpace_NotNull()
        {
            var target = "A string";

            target.ThrowIfNullOrWhiteSpace(nameof(target));
        }

        [TestMethod]
        public void ThrowIfSmallerOrEqual_Zero()
        {
            var target = 0;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => target.ThrowIfSmallerOrEqual(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfSmallerOrEqual_MinusOne()
        {
            var target = -1;

            Assert.ThrowsException<ArgumentOutOfRangeException>(() => target.ThrowIfSmallerOrEqual(nameof(target)));
        }

        [TestMethod]
        public void ThrowIfSmallerOrEqual_One()
        {
            var target = 1;

            target.ThrowIfSmallerOrEqual(nameof(target));
        }
    }
}