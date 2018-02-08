// ———————————————————————————————
// <copyright file="DependencyResolverExtensionsTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for the dependency resolver extensions.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests.Extensions
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Http.Dependencies;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [ExcludeFromCodeCoverage]
    [TestClass]
    [TestCategory("Unit")]
    public class DependencyResolverExtensionsTests
    {
        [TestMethod]
        public void GetService()
        {
            var profile = new Profile();
            var mockedScope = new Mock<IDependencyScope>();

            mockedScope.Setup(s => s.GetService(typeof(Profile))).Returns(profile);

            var actual = mockedScope.Object.GetService<Profile>();

            actual.Should().Be(profile);
        }

        [TestMethod]
        public void GetService_Missing_Scope()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ((IDependencyScope)null).GetService<Profile>());
        }
    }
}