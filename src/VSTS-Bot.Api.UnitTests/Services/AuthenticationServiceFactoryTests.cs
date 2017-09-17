// <copyright file="AuthenticationServiceFactoryTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for AuthenticationServiceFactory class
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests.Services
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using TSBot;

    [TestClass]
    public class AuthenticationServiceFactoryTests
    {
        [TestMethod]
        public void GetServiceTest()
        {
            var sut = new AuthenticationServiceFactory();

            Assert.IsNotNull(sut);
            Assert.ThrowsException<ArgumentNullException>(() => sut.GetService(null));
            Assert.IsNotNull(sut.GetService(new Mock<IVstsApplication>().Object));
        }
    }
}