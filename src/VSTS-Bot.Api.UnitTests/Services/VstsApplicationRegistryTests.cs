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

            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(null, appSecret, appScope));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, null, appScope));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistry(appId, appSecret, null));
        }

        [TestMethod]
        public void GetVstsApplicationRegistrationTest()
        {
            var sut = new VstsApplicationRegistry("myappId", "myappSecret", "myappScope");

            Assert.ThrowsException<ArgumentNullException>(() => sut.GetVstsApplicationRegistration(null));

            var actual = sut.GetVstsApplicationRegistration(new VstsApplicationRegistrationKey("channelId", "userId"));
            Assert.IsNotNull(actual);
        }
    }
}