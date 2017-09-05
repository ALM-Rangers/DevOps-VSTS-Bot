// <copyright file="VstsApplicationTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for VstsApplicationTests class
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests.Services
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VstsApplicationTests
    {
        [TestMethod]
        public void VstsApplicationConstructorTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(null, "secret", "scope"));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication("id", null, "scope"));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication("id", "secret", null));
            Assert.IsNotNull(new VstsApplication("id", "secret", "scope"));
        }

        [TestMethod]
        public void VstsApplicationPropertiesTest()
        {
            var sut = new VstsApplication("id", "secret", "scope");

            Assert.AreEqual("id", sut.Id);
            Assert.AreEqual("secret", sut.Secret);
            Assert.AreEqual("scope", sut.Scope);
        }
    }
}