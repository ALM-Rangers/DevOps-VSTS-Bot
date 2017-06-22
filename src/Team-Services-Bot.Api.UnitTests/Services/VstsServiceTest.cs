// <copyright file="VstsServiceTest.cs" company="PlaceholderCompany">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains tests for VstsService class
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests.Services
{
    using System;
    using System.Threading.Tasks;
    using Common.Tests;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="VstsService"/>.
    /// </summary>
    [TestClass]
    [TestCategory(TestCategories.Unit)]
    public class VstsServiceTest
    {
        private readonly OAuthToken token = new OAuthToken { AccessToken = @"x25onorum4neacdjmvzvaxjeosik7qxo7fbnn6lebefeday7fxmq" };

        /// <summary>
        /// Tests <see cref="VstsService.GetProfile"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetProfileTest()
        {
            var service = new VstsService();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProfile(null));

            // TODO: Implement mocking of VSTS calls and test real behavior
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetAccounts"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetAccountsTest()
        {
            var service = new VstsService();

            var memberId = Guid.NewGuid();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.GetAccounts(null, memberId));

            // TODO: Implement mocking of VSTS calls and test real behavior
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetProjects"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetProjectsTest()
        {
            var service = new VstsService();

            var account = "anaccount";

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(account, null));

            // TODO: Implement mocking of VSTS calls and test real behavior
        }

        [TestMethod]
        public async Task GetBuildDefinitionsTest()
        {
            var service = new VstsService();
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitions(null, "myaccount", this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitions("myproject", null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitions("myproject", "myaccount", null));
        }
    }
}
