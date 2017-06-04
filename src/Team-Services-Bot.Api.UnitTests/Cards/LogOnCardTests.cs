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
    using Common.Tests;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using TSBot.Cards;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class LogOnCardTests
    {
        [TestMethod]
        public void Constructor_Missing_AppId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(null, null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_AuthorizeUrl()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_ChannelId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", new Uri("https://someurl.com"), null, null));
        }

        [TestMethod]
        public void Constructor_Missing_UserId()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard("appId", new Uri("https://someurl.com"), ChannelIds.Skype, null));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "target", Justification = "Need to test the constructor.")]
        [TestMethod]
        public void Constructor()
        {
            var target = new LogOnCard("appId", new Uri("https://someurl.com"), ChannelIds.Skype, "userId");
            target = new LogOnCard("appId", new Uri("https://someurl.com"), ChannelIds.Msteams, "userId");
        }
    }
}