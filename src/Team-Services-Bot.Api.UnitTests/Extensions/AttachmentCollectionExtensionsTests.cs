// ———————————————————————————————
// <copyright file="AttachmentCollectionExtensionsTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the Attachment Collection Extensions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class AttachmentCollectionExtensionsTests
    {
        [TestMethod]
        public void Add_No_HeroCard()
        {
            var attachments = new List<Attachment>();
            Assert.ThrowsException<ArgumentNullException>(() => attachments.Add(null as HeroCard));
        }

        [TestMethod]
        public void Add_No_Attachment_For_HeroCard()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IList<Attachment>)null).Add(null as HeroCard));
        }

        [TestMethod]
        public void Add_No_SignInCard()
        {
            var attachments = new List<Attachment>();
            Assert.ThrowsException<ArgumentNullException>(() => attachments.Add(null as SigninCard));
        }

        [TestMethod]
        public void Add_No_Attachment_For_SignInCard()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IList<Attachment>)null).Add(null as SigninCard));
        }

        [TestMethod]
        public void Add_HeroCard()
        {
            var attachments = new List<Attachment>();
            attachments.Add(new HeroCard());

            attachments.Should().HaveCount(1);
        }

        [TestMethod]
        public void Add_SignInCard()
        {
            var attachments = new List<Attachment>();
            attachments.Add(new SigninCard());

            attachments.Should().HaveCount(1);
        }
    }
}