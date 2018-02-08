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
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.VisualStudio.Services.Account;
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
        public void Constructor_Missing_AppId()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var vstsServiceMock = new Mock<IVstsService>();

            Assert.ThrowsException<ArgumentNullException>(() => new AuthorizeController(null, new Uri("http://authorize.url"), authenticationServiceMock.Object, botDataFactoryMock.Object, vstsServiceMock.Object));
        }

        [TestMethod]
        public void Constructor_Missing_AuthorizeUrl()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var vstsServiceMock = new Mock<IVstsService>();

            Assert.ThrowsException<ArgumentNullException>(() => new AuthorizeController("appId", null, authenticationServiceMock.Object, botDataFactoryMock.Object, vstsServiceMock.Object));
        }

        [TestMethod]
        public void Constructor_Missing_AuthenticationService()
        {
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var vstsServiceMock = new Mock<IVstsService>();

            Assert.ThrowsException<ArgumentNullException>(() => new AuthorizeController("appId", new Uri("http://authorize.url"), null, botDataFactoryMock.Object, vstsServiceMock.Object));
        }

        [TestMethod]
        public void Constructor_Missing_BotDataFactory()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var vstsServiceMock = new Mock<IVstsService>();

            Assert.ThrowsException<ArgumentNullException>(() => new AuthorizeController("appId", new Uri("http://authorize.url"), authenticationServiceMock.Object, null, vstsServiceMock.Object));
        }

        [TestMethod]
        public void Constructor_Missing_VstsService()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();

            Assert.ThrowsException<ArgumentNullException>(() => new AuthorizeController("appId", new Uri("http://authorize.url"), authenticationServiceMock.Object, botDataFactoryMock.Object, null));
        }

        [TestMethod]
        public async Task Authorize_No_Code()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var vstsServiceMock = new Mock<IVstsService>();

            var target = new AuthorizeController("appId", new Uri("http://authorize.url"), authenticationServiceMock.Object, botDataFactoryMock.Object, vstsServiceMock.Object);
            var result = await target.Index(null, null, null) as ViewResult;
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ViewData.ModelState.First(pair => pair.Value.Errors.Any()));
        }

        [TestMethod]
        public async Task Authorize_No_State()
        {
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var vstsServiceMock = new Mock<IVstsService>();

            var target = new AuthorizeController("appId", new Uri("http://authorize.url"), authenticationServiceMock.Object, botDataFactoryMock.Object, vstsServiceMock.Object);
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
            var authenticationServiceMock = new Mock<IAuthenticationService>();
            var botDataFactoryMock = new Mock<IBotDataFactory>();
            var botData = new Mock<IBotData>();
            var botDataBag = new Mock<IBotDataBag>();
            var vstsServiceMock = new Mock<IVstsService>();

            var token = new OAuthToken();
            var profile = new Microsoft.VisualStudio.Services.Profile.Profile { DisplayName = "UniqueName" };
            var accounts = new List<Account> { new Account(Guid.NewGuid()) { AccountName = "Account1" } };

            var target = new AuthorizeController("appId", new Uri("http://authorize.url"), authenticationServiceMock.Object, botDataFactoryMock.Object, vstsServiceMock.Object);

            const string code = "1234567890";
            const string state = "channel1;user1";
            var data = new UserData { Pin = "12345" };

            authenticationServiceMock
                .Setup(a => a.GetToken("appId", new Uri("http://authorize.url"), code))
                .ReturnsAsync(() => token);

            vstsServiceMock
                .Setup(p => p.GetProfile(token))
                .ReturnsAsync(profile);

            vstsServiceMock
                .Setup(p => p.GetAccounts(token, It.IsAny<Guid>()))
                .ReturnsAsync(accounts);

            botDataFactoryMock
                .Setup(b => b.Create(It.Is<Address>(a =>
                    a.ChannelId.Equals("channel1", StringComparison.Ordinal) &&
                    a.UserId.Equals("user1", StringComparison.Ordinal))))
                .Returns(botData.Object);

            botData
                .Setup(bd => bd.UserData)
                .Returns(botDataBag.Object);

            botDataBag
                .Setup(bd => bd.TryGetValue("userData", out data))
                .Returns(true);

            var result = await target.Index(code, string.Empty, state) as ViewResult;

            botDataBag
                .Verify(bd => bd.SetValue("userData", data));

            result.Should().NotBeNull();
            ((Authorize)result.Model).Pin.Should().Be(data.Pin);
            data.Profiles.Should().Contain(p => p.Id.Equals(profile.Id));
        }
    }
}