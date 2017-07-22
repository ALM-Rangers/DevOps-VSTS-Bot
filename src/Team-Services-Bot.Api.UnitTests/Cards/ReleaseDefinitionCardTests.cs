// ———————————————————————————————
// <copyright file="ReleaseDefinitionCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the ReleaseDefinitionCard.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using System.Linq;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class ReleaseDefinitionCardTests
    {
        [TestMethod]
        public void Constructor_Missing_ReleaseDefinition()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ReleaseDefinitionCard(null));
        }

        [TestMethod]
        public void Constructor()
        {
            var releaseDefinition = new ReleaseDefinition { Id = 1, Name = "Release 1" };

            var target = new ReleaseDefinitionCard(releaseDefinition);
            target.Title.Should().Be(releaseDefinition.Name);

            target.Buttons.First().Value.Should().Be("create 1");
        }
    }
}