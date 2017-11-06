// ———————————————————————————————
// <copyright file="BotFactoryTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains Tests for the BotFactory.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Factories
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory("Unit")]
    [ExcludeFromCodeCoverage]
    public class BotFactoryTests
    {
        [TestMethod]
        public void Constructor()
        {
            var mockedStore = new Mock<IBotDataStore<BotData>>();

            var target = new BotDataFactory(mockedStore.Object);
        }

        [TestMethod]
        public void Constructor_Missing_Store()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BotDataFactory(null));
        }

        [TestMethod]
        public void Create_Missing_Address()
        {
            var mockedStore = new Mock<IBotDataStore<BotData>>();

            var target = new BotDataFactory(mockedStore.Object);

            Assert.ThrowsException<ArgumentNullException>(() => target.Create(null));
        }

        [TestMethod]
        public void Create()
        {
            var address = new Address(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
            var mockedStore = new Mock<IBotDataStore<BotData>>();

            var target = new BotDataFactory(mockedStore.Object);

            var actual = target.Create(address);

            actual.Should().BeOfType<JObjectBotData>();
        }
    }
}