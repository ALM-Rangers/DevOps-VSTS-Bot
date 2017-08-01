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
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
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
        public void GetAccount_Missing_IBotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetAccount());
        }

        [TestMethod]
        public void GetAccount_No_Account()
        {
            string account;
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Account", out account)).Returns(false).Verifiable();

            var result = dataBag.Object.GetAccount();

            dataBag.Verify();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void GetAccount()
        {
            var account = "account1";
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("Account", out account)).Returns(true).Verifiable();

            var result = dataBag.Object.GetAccount();

            dataBag.Verify();

            result.Should().Be(account);
        }

        [TestMethod]
        public void GetTeamProject_Missing_IBotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetTeamProject());
        }

        [TestMethod]
        public void GetTeamProject_No_TeamProject()
        {
            string teamProject;
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("TeamProject", out teamProject)).Returns(false).Verifiable();

            var result = dataBag.Object.GetTeamProject();

            dataBag.Verify();

            result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void GetTeamProject()
        {
            var teamProject = "TeamProject1";
            var dataBag = new Mock<IBotDataBag>();

            dataBag.Setup(d => d.TryGetValue("TeamProject", out teamProject)).Returns(true).Verifiable();

            var result = dataBag.Object.GetTeamProject();

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
        public void GetNotValidatedByPinProfile_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetNotValidatedByPinProfile());
        }

        [TestMethod]
        public void GetNotValidatedByPinProfile_BotDataBag_No_Profile()
        {
            VstsProfile profile;

            var mocked = new Mock<IBotDataBag>();
            mocked.Setup(m => m.TryGetValue("NotValidatedByPinProfile", out profile)).Returns(false).Verifiable();

            var result = mocked.Object.GetNotValidatedByPinProfile();

            mocked.Verify();

            result.Should().BeNull();
        }

        [TestMethod]
        public void GetNotValidatedByPinProfile_BotDataBag()
        {
            VstsProfile profile = new VstsProfile();

            var mocked = new Mock<IBotDataBag>();
            mocked.Setup(m => m.TryGetValue("NotValidatedByPinProfile", out profile)).Returns(true).Verifiable();

            var result = mocked.Object.GetNotValidatedByPinProfile();

            mocked.Verify();

            result.Should().Be(profile);
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
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).GetProfile());
        }

        [TestMethod]
        public void GetProfile_NoProfile()
        {
            VstsProfile profile;

            var authenticationService = new Mock<IAuthenticationService>();
            var mocked = new Mock<IBotDataBag>();

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => authenticationService.Object)
                .As<IAuthenticationService>();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(false).Verifiable();

            var result = mocked.Object.GetProfile();

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

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => authenticationService.Object)
                .As<IAuthenticationService>();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(true).Verifiable();
            authenticationService.Setup(a => a.GetToken(profile.Token)).ReturnsAsync(newToken).Verifiable();

            var result = mocked.Object.GetProfile();

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

            var builder = new ContainerBuilder();
            builder
                .Register((c, x) => authenticationService.Object)
                .As<IAuthenticationService>();

            var container = builder.Build();
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            mocked.Setup(m => m.TryGetValue("Profile", out profile)).Returns(true).Verifiable();

            var result = mocked.Object.GetProfile();

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
        public void SetAccount_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).SetAccount(null));
        }

        [TestMethod]
        public void SetAccount_BotData_Missing_Account()
        {
            var data = new BotData();
            Assert.ThrowsException<ArgumentNullException>(() => data.SetAccount(null));
        }

        [TestMethod]
        public void SetAccount_BotData()
        {
            var account = "Account1";
            var data = new BotData();

            data.SetAccount(account);

            var result = data.GetProperty<string>("Account");

            result.Should().Be(account);
        }

        [TestMethod]
        public void SetAccount_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).SetAccount(null));
        }

        [TestMethod]
        public void SetAccount_BotDataBag_Missing_Account()
        {
            var mocked = new Mock<IBotDataBag>();
            Assert.ThrowsException<ArgumentNullException>(() => mocked.Object.SetAccount(null));
        }

        [TestMethod]
        public void SetAccount_BotDataBag()
        {
            var account = "Account1";
            var mocked = new Mock<IBotDataBag>();

            mocked.Object.SetAccount(account);

            mocked.Verify(m => m.SetValue("Account", account));
        }

        [TestMethod]
        public void SetNotValidatedByPinProfile_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).SetNotValidatedByPinProfile(null));
        }

        [TestMethod]
        public void SetNotValidatedByPinProfile_BotData_Missing_Profile()
        {
            var data = new BotData();
            Assert.ThrowsException<ArgumentNullException>(() => data.SetNotValidatedByPinProfile(null));
        }

        [TestMethod]
        public void SetNotValidatedByPinProfile_BotData()
        {
            var profile = new VstsProfile();
            var data = new BotData();

            data.SetNotValidatedByPinProfile(profile);

            var result = data.GetProperty<VstsProfile>("NotValidatedByPinProfile");

            result.Should().BeOfType<VstsProfile>();
        }

        [TestMethod]
        public void SetProfile_Missing_BotData()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((BotData)null).SetProfile(null));
        }

        [TestMethod]
        public void SetProfile_BotData_Missing_Profile()
        {
            var data = new BotData();
            Assert.ThrowsException<ArgumentNullException>(() => data.SetProfile(null));
        }

        [TestMethod]
        public void SetProfile_BotData()
        {
            var profile = new VstsProfile();
            var data = new BotData();

            data.SetProfile(profile);

            var result = data.GetProperty<VstsProfile>("Profile");

            result.Should().BeOfType<VstsProfile>();
        }

        [TestMethod]
        public void SetProfile_Missing_BotDataBag()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IBotDataBag)null).SetProfile(null));
        }

        [TestMethod]
        public void SetProfile_BotDataBag_Missing_Account()
        {
            var mocked = new Mock<IBotDataBag>();
            Assert.ThrowsException<ArgumentNullException>(() => mocked.Object.SetProfile(null));
        }

        [TestMethod]
        public void SetProfile_BotDataBag()
        {
            var profile = new VstsProfile();
            var mocked = new Mock<IBotDataBag>();

            mocked.Object.SetProfile(profile);

            mocked.Verify(m => m.SetValue("Profile", profile));
        }
    }
}