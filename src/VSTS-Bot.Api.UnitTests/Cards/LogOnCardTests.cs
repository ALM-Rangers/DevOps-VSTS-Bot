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
        public void Constructor_Missing_AppId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(null, "appScope", new Uri("https://authorize.url"), ChannelIds.Skype, "userId"));
        }

        [TestMethod]
        public void Constructor_Missing_AppScope()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", null, new Uri("https://authorize.url"), ChannelIds.Skype, "userId"));
        }

        [TestMethod]
        public void Constructor_Missing_AuthorizeUrl()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", "appScope", null, ChannelIds.Skype, "userId"));
        }

        [TestMethod]
        public void Constructor_Missing_ChannelId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", "appScope", new Uri("https://authorize.url"), null, "userId"));
        }

        [TestMethod]
        public void Constructor_Missing_UserId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", "appScope", new Uri("https://authorize.url"), ChannelIds.Skype, null));
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "target", Justification = "Test code, nothing to worry about.")]
        [TestMethod]
        public void Constructor()
        {
            var target = new LogOnCard("appId", "appScope", new Uri("https://authorize.url"), ChannelIds.Skype, "userId");
            target = new LogOnCard("appId", "appScope", new Uri("https://authorize.url"), ChannelIds.Msteams, "userId");
        }
    }
}