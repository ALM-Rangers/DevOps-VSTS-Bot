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
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit tests for <see cref="VstsService"/>.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
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

            // TODO: Implement mocking of VSTS calls
            // var profile = await service.GetProfile(this.token);
            // Assert.IsNotNull(profile);
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

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetAccounts(null, memberId));

            // TODO: Implement mocking of VSTS calls
            // IList<Account> accounts = await service.GetAccounts(this.token, memberId);
            // Assert.IsNotNull(accounts);
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

            var accountUrl = new Uri("https://fakeaccount.visualstudio.com");

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(accountUrl, null));

            // TODO: Implement mocking of VSTS calls
            // IEnumerable<TeamProjectReference> result = await service.GetProjects(accountUrl, this.token);
            // Assert.IsNotNull(result);
        }
    }
}
