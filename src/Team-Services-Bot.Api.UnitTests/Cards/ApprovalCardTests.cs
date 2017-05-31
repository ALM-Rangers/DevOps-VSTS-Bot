// ———————————————————————————————
// <copyright file="ApprovalCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// $SUMMARY$
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [ExcludeFromCodeCoverage]
    [TestClass]
    public class ApprovalCardTests
    {
        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor_Missing_Account()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApprovalCard(null, null, null));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor_Missing_Approval()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApprovalCard("account1", null, null));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor_Missing_TeamProject()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new ApprovalCard("account1", new ReleaseApproval(), null));
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public void Constructor()
        {
            var approval = new ReleaseApproval
            {
                ReleaseDefinitionReference = new ReleaseDefinitionShallowReference { Id = 3, Name = "Release Def 3" },
                ReleaseEnvironmentReference = new ReleaseEnvironmentShallowReference { Id = 2, Name = "DEV" },
                ReleaseReference = new ReleaseShallowReference { Id = 1, Name = "release-1" }
            };

            var target = new ApprovalCard("account1", approval, "teamproject1");

            target.Subtitle.Should().Be("release-1");
            target.Text.Should().Be("DEV");
            target.Title.Should().Be("Release Def 3");
            target.Tap.Type.Should().Be(ActionTypes.OpenUrl);
            target.Tap.Value.Should().Be("https://account1.visualstudio.com/teamproject1/_release?definitionId=3&_a=release-summary&releaseId=1");
        }
    }
}