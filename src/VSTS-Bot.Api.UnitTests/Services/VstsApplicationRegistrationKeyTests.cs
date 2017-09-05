// <copyright file="VstsApplicationRegistrationKeyTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for VstsApplicationRegistrationKeyTests class
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Tests
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class VstsApplicationRegistrationKeyTests
    {
        [TestMethod]
        public void VstsApplicationRegistrationKeyTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistrationKey(null));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistrationKey(null, null));
            Assert.ThrowsException<ArgumentNullException>(() => new VstsApplicationRegistrationKey("channelId", null));
            Assert.IsNotNull(new VstsApplicationRegistrationKey("channelId", "userId"));
        }

        [TestMethod]
        public void ToStringTest()
        {
            var sut = new VstsApplicationRegistrationKey("channelId", "userId");

            Assert.AreEqual(@"channelId/userId", sut.ToString());
        }
    }
}