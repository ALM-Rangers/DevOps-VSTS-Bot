// ———————————————————————————————
// <copyright file="BuildDefinitionCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the BuildDefinitionsCard.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [TestClass]
    [TestCategory(TestCategories.Unit)]
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
            var buildDefinition = new BuildDefinitionReference { Name = "Build 1" };

            var target = new BuildDefinitionCard(buildDefinition);
            target.Title.Should().Be(buildDefinition.Name);
        }
    }
}