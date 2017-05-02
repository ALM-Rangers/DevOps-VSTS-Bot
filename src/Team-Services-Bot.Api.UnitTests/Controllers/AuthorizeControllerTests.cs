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
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    /// <summary>
    /// Contains the tests for the Authorize Controller.
    /// </summary>
    [TestClass]
    public class AuthorizeControllerTests
    {
        /// <summary>
        /// Tests the normal Authorization flow.
        /// </summary>
        /// <returns>.</returns>
        [TestMethod]
        public async Task AuthorizeTest()
        {
            var authenticationService = new Mock<IAuthenticationService>();
            var botService = new Mock<IBotService>();
            var profileService = new Mock<IProfileService>();

            var token = new OAuthToken();
            var profile = new Profile();
            var accounts = new List<Account>();
            var botData = new BotData();

            var controller = new AuthorizeController(
                botService.Object,
                new TelemetryClient(),
                authenticationService.Object,
                profileService.Object);

            var code = "1234567890";
            var state = "channel1;user1";

            authenticationService
                .Setup(a => a.GetToken(code))
                .ReturnsAsync(() => token);
            profileService
                .Setup(p => p.GetProfile(token))
                .ReturnsAsync(profile);
            profileService
                .Setup(p => p.GetAccounts(token, It.IsAny<Guid>()))
                .ReturnsAsync(accounts);
            botService
                .Setup(b => b.GetUserData("channel1", "user1"))
                .ReturnsAsync(botData);
            botService
                .Setup(b => b.SetUserData("channel1", "user1", botData))
                .Returns(Task.CompletedTask);

            var result = await controller.Index(code, string.Empty, state) as ViewResult;
            var profiles = botData.GetProfiles();

            Assert.IsNotNull(result);
            Assert.IsNotNull(profiles);
            Assert.AreEqual(1, profiles.Count);
        }
    }
}