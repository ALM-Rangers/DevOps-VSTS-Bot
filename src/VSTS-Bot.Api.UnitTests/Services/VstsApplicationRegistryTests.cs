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

            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(null, appSecret, appScope, redirectUrl));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, null, appScope, redirectUrl));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, appSecret, null, redirectUrl));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, appSecret, appScope, null));
        }

        [TestMethod]
        public void GetVstsApplicationRegistrationTest()
        {
            var sut = new VstsApplicationRegistry("myappId", "myappSecret", "myappScope", new Uri("https://localjost/redirectcallback"));

            Assert.ThrowsException<ArgumentNullException>(() => sut.GetVstsApplicationRegistration(null));

            var actual = sut.GetVstsApplicationRegistration(new VstsApplicationRegistrationKey("channelId", "userId"));
            Assert.IsNotNull(actual);
        }
    }
}