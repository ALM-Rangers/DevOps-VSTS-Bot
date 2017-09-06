// ———————————————————————————————
// <copyright file="AuthorizeControllerTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the AuthorizeController.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Tests for <see cref="AuthorizeController"/> class
    /// </summary>
    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class AuthorizeControllerTests
    {
        [TestMethod]
        public async Task Authorize_No_Code()
        {
            var applicationRegistry = new Mock<IVstsApplicationRegistry>();
            var botService = new Mock<IBotService>();
            var vstsService = new Mock<IVstsService>();

            var target = new AuthorizeController(botService.Object, applicationRegistry.Object, vstsService.Object);
            var result = await target.Index(null, null, null) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.ModelState.First(pair => pair.Value.Errors.Any()));
        }

        [TestMethod]
        public async Task Authorize_No_State()
        {
            var applicationRegistry = new Mock<IVstsApplicationRegistry>();
            var botService = new Mock<IBotService>();
            var vstsService = new Mock<IVstsService>();

            var target = new AuthorizeController(botService.Object, applicationRegistry.Object, vstsService.Object);
            var result = await target.Index("123567890", null, null) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.ModelState.First(pair => pair.Value.Errors.Any()));
        }

        /// <summary>
        /// Test of valid authorization
        /// </summary>
        /// <returns><see cref="Task"/> object.</returns>
        [TestMethod]
        public async Task Authorize_A_Valid_LogOn()
        {
            var authenticationService = new Mock<IAuthenticationService>();
            var application = new Mock<IVstsApplication>();
            var applicationRegistry = new Mock<IVstsApplicationRegistry>();
            var botService = new Mock<IBotService>();
            var vstsService = new Mock<IVstsService>();

            var token = new OAuthToken();
            var profile = new Profile();
            var accounts = new List<Account> { new Account(Guid.NewGuid()) { AccountName = "Account1" } };
            var botData = new BotData();

            var target = new AuthorizeController(botService.Object, applicationRegistry.Object, vstsService.Object);

            const string code = "1234567890";
            const string pin = "12345";
            const string state = "channel1;user1";

            botData.SetProperty("Pin", pin);

            authenticationService
                .Setup(a => a.GetToken(code))
                .ReturnsAsync(() => token);

            application
                .Setup(vstsApplication => vstsApplication.AuthenticationService)
                .Returns(authenticationService.Object);

            applicationRegistry
                .Setup(registry => registry.GetVstsApplicationRegistration(It.IsAny<VstsApplicationRegistrationKey>()))
                .Returns(application.Object);

            vstsService
                .Setup(p => p.GetProfile(token))
                .ReturnsAsync(profile);

            vstsService
                .Setup(p => p.GetAccounts(token, It.IsAny<Guid>()))
                .ReturnsAsync(accounts);

            botService
                .Setup(b => b.GetUserData("channel1", "user1"))
                .ReturnsAsync(botData);

            botService
                .Setup(b => b.SetUserData("channel1", "user1", botData))
                .Returns(Task.CompletedTask);

            var result = await target.Index(code, string.Empty, state) as ViewResult;
            var vstsProfile = botData.GetProperty<VstsProfile>("NotValidatedByPinProfile");

            result.Should().NotBeNull();
            vstsProfile.Should().NotBeNull();
            ((Authorize)result.Model).Pin.Should().Be(pin);
        }
    }
}