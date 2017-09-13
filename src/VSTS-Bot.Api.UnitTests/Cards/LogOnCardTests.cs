// ———————————————————————————————
// <copyright file="LogOnCardTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the LogOnCard.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Cards
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using TSBot.Cards;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class LogOnCardTests
    {
        [TestMethod]
        public void Constructor_Missing_VstsApplication()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_ChannelId()
        {
            var factory = new Mock<IAuthenticationServiceFactory>().Object;
            var application = new VstsApplication("id", "secret", "scope", new Uri("http://localhost/redirect"), factory);
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_UserId()
        {
            var factory = new Mock<IAuthenticationServiceFactory>().Object;
            var application = new VstsApplication("id", "secret", "scope", new Uri("http://localhost/redirect"), factory);
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, ChannelIds.Skype, null));
        }

        [TestMethod]
        public void Constructor()
        {
            var factory = new Mock<IAuthenticationServiceFactory>().Object;
            var application = new VstsApplication("id", "secret", "scope", new Uri("http://localhost/redirect"), factory);
            Assert.IsNotNull(new LogOnCard(application, ChannelIds.Skype, "userId"));
            Assert.IsNotNull(new LogOnCard(application, ChannelIds.Msteams, "userId"));
        }
    }
}