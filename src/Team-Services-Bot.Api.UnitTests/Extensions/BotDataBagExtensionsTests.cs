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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
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

        [TestMethod]
        public void GetPin_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).GetPin());
        }

        [TestMethod]
        public void GetPin_BotData_No_Pin()
        {
            var target = new BotData();
            var result = target.GetPin();

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetPin_BotData()
        {
            var target = new BotData();
            target.SetProperty("Pin", "12345");

            var result = target.GetPin();

            result.Should().Be("12345");
        }

        [TestMethod]
        public void GetPin_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetPin());
        }

        [TestMethod]
        public void GetPin_BotDataBag_No_Pin()
        {
            string pin;

            var mocked = new Mock<IBotDataBag>();
            mocked.Setup(m => m.TryGetValue("Pin", out pin)).Returns(false).Verifiable();

            var result = mocked.Object.GetPin();

            mocked.Verify();

            result.Should().BeEmpty();
        }

        [TestMethod]
        public void GetPin_BotDataBag()
        {
            var pin = "12345";

            var mocked = new Mock<IBotDataBag>();
            mocked.Setup(m => m.TryGetValue("Pin", out pin)).Returns(true).Verifiable();

            var result = mocked.Object.GetPin();

            mocked.Verify();

            result.Should().Be(pin);
        }

        [TestMethod]
        public void GetProfile_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetProfile(null));
        }

        [TestMethod]
        public void GetProfile_Missing_AuthenticationService()
        {
            var mocked = new Mock<IBotDataBag>();

            Assert.ThrowsException<ArgumentNullException>(() => mocked.Object.GetProfile(null));
        }

        [TestMethod]
        public void GetProfile_NoProfile()
        {
            VstsProfile profile;

            var authenticationService = new Mock<IAuthenticationService>();
            var mocked = new Mock<IBotDataBag>();

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(false).Verifiable();

            var result = mocked.Object.GetProfile(authenticationService.Object);

            mocked.Verify();

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetProfile_ExpiredToken()
        {
            var profile = new VstsProfile { Token = new OAuthToken { CreatedOn = DateTime.UtcNow.AddHours(-2), ExpiresIn = 3600, RefreshToken = "1111" } };
            var newToken = new OAuthToken();

            var authenticationService = new Mock<IAuthenticationService>();
            var mocked = new Mock<IBotDataBag>();

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(true).Verifiable();
            authenticationService.Setup(a => a.GetToken(profile.Token)).ReturnsAsync(newToken).Verifiable();

            var result = mocked.Object.GetProfile(authenticationService.Object);

            mocked.Verify();
            authenticationService.Verify();
            result.Should().Be(profile);
            result.Token.Should().Be(newToken);
        }

        [TestMethod]
        public void GetProfile()
        {
            var profile = new VstsProfile { Token = new OAuthToken { CreatedOn = DateTime.UtcNow, ExpiresIn = 3600, RefreshToken = "1111" } };

            var authenticationService = new Mock<IAuthenticationService>();
            var mocked = new Mock<IBotDataBag>();

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(true).Verifiable();

            var result = mocked.Object.GetProfile(authenticationService.Object);

            mocked.Verify();
            result.Should().Be(profile);
            result.Token.Should().Be(profile.Token);
        }

        [TestMethod]
        public void GetProfiles_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetProfiles());
        }

        [TestMethod]
        public void GetProfiles_Missing_Profiles()
        {
            var mocked = new Mock<IBotDataBag>();
            var result = mocked.Object.GetProfiles();

            result.Should().HaveCount(0);
        }

        [TestMethod]
        public void GetProfiles()
        {
            var profiles = new List<VstsProfile> { new VstsProfile() } as IList<VstsProfile>;

            var mocked = new Mock<IBotDataBag>();

            mocked.Setup(m => m.TryGetValue("Profiles", out profiles)).Returns(true);

            var result = mocked.Object.GetProfiles();

            Assert.AreEqual(profiles, result);
        }

        [TestMethod]
        public void SetCurrentAccount_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).SetCurrentAccount(null));
        }

        [TestMethod]
        public void SetCurrentAccount_BotData_Missing_Account()
        {
            var data = new BotData();
            Assert.ThrowsException<ArgumentNullException>(() => data.SetCurrentAccount(null));
        }

        [TestMethod]
        public void SetCurrentAccount_BotData()
        {
            var account = "Account1";
            var data = new BotData();

            data.SetCurrentAccount(account);

            var result = data.GetProperty<string>("Account");

            result.Should().Be(account);
        }

        [TestMethod]
        public void SetCurrentAccount_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).SetCurrentAccount(null));
        }

        [TestMethod]
        public void SetCurrentAccount_BotDataBag_Missing_Account()
        {
            var mocked = new Mock<IBotDataBag>();
            Assert.ThrowsException<ArgumentNullException>(() => mocked.Object.SetCurrentAccount(null));
        }

        [TestMethod]
        public void SetCurrentAccount_BotDataBag()
        {
            var account = "Account1";
            var mocked = new Mock<IBotDataBag>();

            mocked.Object.SetCurrentAccount(account);

            mocked.Verify(m => m.SetValue("Account", account));
        }

        [TestMethod]
        public void SetCurrentProfile_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).SetCurrentProfile(null));
        }

        [TestMethod]
        public void SetCurrentProfile_BotData_Missing_Account()
        {
            var data = new BotData();
            Assert.ThrowsException<ArgumentNullException>(() => data.SetCurrentProfile(null));
        }

        [TestMethod]
        public void SetCurrentProfile_BotData()
        {
            var profile = new VstsProfile();
            var data = new BotData();

            data.SetCurrentProfile(profile);

            var result = data.GetProperty<VstsProfile>("Profile");

            result.Should().BeOfType<VstsProfile>();
        }

        [TestMethod]
        public void SetCurrentProfile_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).SetCurrentProfile(null));
        }

        [TestMethod]
        public void SetCurrentProfile_BotDataBag_Missing_Account()
        {
            var mocked = new Mock<IBotDataBag>();
            Assert.ThrowsException<ArgumentNullException>(() => mocked.Object.SetCurrentProfile(null));
        }

        [TestMethod]
        public void SetCurrentProfile_BotDataBag()
        {
            var profile = new VstsProfile();
            var mocked = new Mock<IBotDataBag>();

            mocked.Object.SetCurrentProfile(profile);

            mocked.Verify(m => m.SetValue("Profile", profile));
        }
    }
}