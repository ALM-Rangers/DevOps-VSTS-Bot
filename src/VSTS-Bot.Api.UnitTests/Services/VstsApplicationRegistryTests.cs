// <copyright file="VstsApplicationRegistryTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for VstsApplicationRegistry class
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests.Services
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    public class VstsApplicationRegistryTests
    {
        [TestMethod]
        public void VstsApplicationRegistryConstructorTest()
        {
            var appId = "myappId";
            var appSecret = "myappSecret";
            var appScope = "myappScope";
            var redirectUrl = new Uri("https://localjost/redirectcallback");
            var factory = new Mock<IAuthenticationServiceFactory>().Object;

            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(null, appSecret, appScope, redirectUrl, factory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, null, appScope, redirectUrl, factory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, appSecret, null, redirectUrl, factory));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, appSecret, appScope, null, factory));
        }

        [TestMethod]
        public void GetVstsApplicationRegistrationTest()
        {
            var sut = new VstsApplicationRegistry(
                "myappId",
                "myappSecret",
                "myappScope",
                new Uri("https://localjost/redirectcallback"),
                new Mock<IAuthenticationServiceFactory>().Object);

            Assert.ThrowsException<ArgumentNullException>(() => sut.GetVstsApplicationRegistration(null));

            var actual = sut.GetVstsApplicationRegistration("userId");
            Assert.IsNotNull(actual);
        }
    }
}