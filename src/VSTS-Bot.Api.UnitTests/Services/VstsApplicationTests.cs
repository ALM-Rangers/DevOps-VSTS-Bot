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
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(null, "secret", "scope", new Uri("http://localhost/redirect")));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication("id", null, "scope", new Uri("http://localhost/redirect")));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication("id", "secret", null, new Uri("http://localhost/redirect")));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication("id", "secret", "scope", null));
            Assert.IsNotNull(new VstsApplication("id", "secret", "scope", new Uri("http://localhost/redirect")));
        }

        [TestMethod]
        public void VstsApplicationPropertiesTest()
        {
            var sut = new VstsApplication("id", "secret", "scope", new Uri("http://localhost/redirect"));

            Assert.AreEqual("id", sut.Id);
            Assert.AreEqual("secret", sut.Secret);
            Assert.AreEqual("scope", sut.Scope);
            Assert.AreEqual(new Uri("http://localhost/redirect"), sut.RedirectUri);
            Assert.IsNotNull(sut.AuthenticationService);
        }
    }
}