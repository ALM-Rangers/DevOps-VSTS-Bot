// ———————————————————————————————
// <copyright file="BotDataBagExtensionsTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the BotDataBag Extensions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class BotDataBagExtensionsTests
    {
        [TestMethod]
        public void GetCurrentAccount_Missing_IBotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetCurrentAccount());
        }

        [TestMethod]
        public void GetCurrentAccount_No_Account()
        {
            string account;
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Account", out account)).Returns(false).Verifiable();

            var result = dataBag.Object.GetCurrentAccount();

            dataBag.Verify();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void GetCurrentAccount()
        {
            var account = "account1";
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Account", out account)).Returns(true).Verifiable();

            var result = dataBag.Object.GetCurrentAccount();

            dataBag.Verify();

            result.Should().Be(account);
        }

        [TestMethod]
        public void GetCurrentProfile_Missing_IBotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetCurrentProfile());
        }

        [TestMethod]
        public void GetCurrentProfile_No_Profile()
        {
            VstsProfile profile;
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Profile", out profile)).Returns(false).Verifiable();

            var result = dataBag.Object.GetCurrentProfile();

            dataBag.Verify();

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetCurrentProfile()
        {
            var profile = new VstsProfile();
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Profile", out profile)).Returns(true).Verifiable();

            var result = dataBag.Object.GetCurrentProfile();

            dataBag.Verify();

            result.Should().Be(profile);
        }

        [TestMethod]
        public void GetCurrentTeamProject_Missing_IBotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetCurrentTeamProject());
        }

        [TestMethod]
        public void GetCurrentTeamProject_No_TeamProject()
        {
            string teamProject;
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("TeamProject", out teamProject)).Returns(false).Verifiable();

            var result = dataBag.Object.GetCurrentTeamProject();

            dataBag.Verify();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void GetCurrentTeamProject()
        {
            var teamProject = "TeamProject1";
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("TeamProject", out teamProject)).Returns(true).Verifiable();

            var result = dataBag.Object.GetCurrentTeamProject();

            dataBag.Verify();

            result.Should().Be(teamProject);
        }
    }
}