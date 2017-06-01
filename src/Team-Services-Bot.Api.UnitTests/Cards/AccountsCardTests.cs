// ———————————————————————————————
// <copyright file="AccountsCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the AccountsCard.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class AccountsCardTests
    {
        [TestMethod]
        public void Constructor_Missing_Accounts()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new AccountsCard(null));
        }

        [TestMethod]
        public void Constructor()
        {
            var accounts = new List<string> { "Account1", "Account2" };

            var target = new AccountsCard(accounts);

            target.Buttons.Should().HaveCount(2);
            target.Buttons[0].Title.Should().Be("Account1");
            target.Buttons[0].Type.Should().Be(ActionTypes.ImBack);
            target.Buttons[0].Value.Should().Be("Account1");
            target.Buttons[1].Title.Should().Be("Account2");
            target.Buttons[1].Type.Should().Be(ActionTypes.ImBack);
            target.Buttons[1].Value.Should().Be("Account2");
        }
    }
}