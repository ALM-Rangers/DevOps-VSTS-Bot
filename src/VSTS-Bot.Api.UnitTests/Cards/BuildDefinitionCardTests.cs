// ———————————————————————————————
// <copyright file="BuildDefinitionCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the BuildDefinitionCard.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [TestClass]
    [TestCategory("Unit")]
    public class BuildDefinitionCardTests
    {
        [TestMethod]
        public void Constructor_Missing_BuildDefinition()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BuildDefinitionCard(null));
        }

        [TestMethod]
        public void Constructor()
        {
            var buildDefinition = new BuildDefinitionReference { Id = 1, Name = "Build 1" };

            var target = new BuildDefinitionCard(buildDefinition);
            target.Title.Should().Be(buildDefinition.Name);

            target.Buttons.First().Value.Should().Be("queue 1");
        }
    }
}