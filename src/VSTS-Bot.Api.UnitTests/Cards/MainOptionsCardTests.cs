// ———————————————————————————————
// <copyright file="MainOptionsCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the main options card.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    /// <summary>
    /// Contains the tests for the main options card.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class MainOptionsCardTests
    {
        /// <summary>
        /// Not connected.
        /// </summary>
        [TestMethod]
        public void NotConnected()
        {
            var card = new MainOptionsCard(false);

            card.Buttons.Should().HaveCount(1);
        }

        /// <summary>
        /// Connected.
        /// </summary>
        [TestMethod]
        public void Connected()
        {
            var card = new MainOptionsCard(true);

            card.Buttons.Should().HaveCount(7);
        }
    }
}