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
    using Autofac;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class VstsApplicationTests
    {
        [TestMethod]
        public void VstsApplicationConstructorTest()
        {
            var id = "id";
            var secret = "secret";
            var scope = "scope";
            var redirectUri = new Uri("http://localhost/redirect");
            var serviceFactory = new Mock<IAuthenticationServiceFactory>().Object;

            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(null, secret, scope, redirectUri, serviceFactory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(id, null, scope, redirectUri, serviceFactory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(id, secret, null, redirectUri, serviceFactory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(id, secret, scope, null, serviceFactory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplication(id, secret, scope, redirectUri, null));
            Assert.IsNotNull(new VstsApplication(id, secret, scope, redirectUri, serviceFactory));
        }

        [TestMethod]
        public void VstsApplicationPropertiesTest()
        {
            var id = "id";
            var secret = "secret";
            var scope = "scope";
            var redirectUri = new Uri("http://localhost/redirect");

            var factoryMock = new Mock<IAuthenticationServiceFactory>();

            factoryMock
                .Setup(factory => factory.GetService(It.IsAny<IVstsApplication>()))
                .Returns(() => new Mock<IAuthenticationService>().Object);

            var sut = new VstsApplication(id, secret, scope, redirectUri, factoryMock.Object);

            Assert.AreEqual(id, sut.Id);
            Assert.AreEqual(secret, sut.Secret);
            Assert.AreEqual(scope, sut.Scope);
            Assert.AreEqual(redirectUri, sut.RedirectUri);
            Assert.IsNotNull(sut.AuthenticationService);
        }
    }
}