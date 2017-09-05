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
    using TSBot.Cards;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class LogOnCardTests
    {
        [TestMethod]
        public void Constructor_Missing_VstsApplication()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(null, null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_AppScope()
        {
            var application = new VstsApplication("id", "secret", "scope");
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_AuthorizeUrl()
        {
            var application = new VstsApplication("id", "secret", "scope");
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, null, null, null));
        }

        [TestMethod]
        public void Constructor_Missing_ChannelId()
        {
            var application = new VstsApplication("id", "secret", "scope");
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, new Uri("https://someurl.com"), null, null));
        }

        [TestMethod]
        public void Constructor_Missing_UserId()
        {
            var application = new VstsApplication("id", "secret", "scope");
            Assert.ThrowsException<ArgumentNullException>(() => new LogOnCard(application, new Uri("https://someurl.com"), ChannelIds.Skype, null));
        }

        [SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "target", Justification = "Need to test the constructor.")]
        [TestMethod]
        public void Constructor()
        {
            var application = new VstsApplication("id", "secret", "scope");
            Assert.IsNotNull(new LogOnCard(application, new Uri("https://someurl.com"), ChannelIds.Skype, "userId"));
            Assert.IsNotNull(new LogOnCard(application, new Uri("https://someurl.com"), ChannelIds.Msteams, "userId"));
        }
    }
}