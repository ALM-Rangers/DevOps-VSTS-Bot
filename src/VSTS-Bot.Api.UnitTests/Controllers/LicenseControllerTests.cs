// ———————————————————————————————
// <copyright file="LicenseControllerTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the LicenseController.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Mvc;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class LicenseControllerTests
    {
        [TestMethod]
        public void Index()
        {
            using (var target = new LicenseController())
            {
                var result = target.Index() as ViewResult;

                result.Should().NotBeNull();
            }
        }
    }
}