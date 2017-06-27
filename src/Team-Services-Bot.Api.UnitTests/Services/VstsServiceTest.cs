// <copyright file="VstsServiceTest.cs">
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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Tests;
    using Microsoft.QualityTools.Testing.Fakes;
    using Microsoft.TeamFoundation.Build.WebApi;
    using Microsoft.TeamFoundation.Build.WebApi.Fakes;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.TeamFoundation.Core.WebApi.Fakes;
    using Microsoft.VisualStudio.Services.Account;
    using Microsoft.VisualStudio.Services.Account.Client;
    using Microsoft.VisualStudio.Services.Account.Client.Fakes;
    using Microsoft.VisualStudio.Services.Profile;
    using Microsoft.VisualStudio.Services.Profile.Client;
    using Microsoft.VisualStudio.Services.Profile.Client.Fakes;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients.Fakes;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Fakes;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.VisualStudio.Services.WebApi.Fakes;
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
        /// Tests <see cref="VstsService.ChangeApprovalStatus"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        [TestMethod]
        [Ignore]
        public async Task ChangeApprovalStatusTest()
        {
            var service = new VstsService();
            string account = "MyAccount";
            string project = "MyProject";
            int id = 1234;
            string comment = "My comment";
            ApprovalStatus status = ApprovalStatus.Undefined;
            VstsProfile profile = new VstsProfile
            {
                Id = Guid.NewGuid(),
                Token = this.token,
                EmailAddress = "me@email.com"
            };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(null, project, profile, id, status, comment));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(account, null, profile, id, status, comment));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(account, project, null, id, status, comment));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(account, project, profile, id, status, null));

            using (ShimsContext.Create())
            {
                ReleaseApproval updatedApproval = null;

                var shimClients = new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProfileHttpClient(new Profile()),
                    new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                    {
                        GetApprovalAsyncStringInt32NullableOfBooleanObjectCancellationToken = (p, i, includeHistory, userState, cancellationToken) => Task.Run(
                            () =>
                            {
                                return new ReleaseApproval { Id = i };
                            },
                            cancellationToken),
                        UpdateReleaseApprovalAsyncReleaseApprovalStringInt32ObjectCancellationToken = (releaseApproval, p, i, userState, cancellationToken) => Task.Run(
                            delegate
                            {
                                Assert.AreEqual(project, p);
                                updatedApproval = releaseApproval;
                                return updatedApproval;
                            },
                            cancellationToken)
                    }.Instance
                };

                InitializeConnectionShim(shimClients);

                await service.ChangeApprovalStatus(account, project, profile, 4, ApprovalStatus.Canceled, comment);

                Assert.IsNotNull(updatedApproval);
                Assert.AreEqual(4, updatedApproval.Id);
                Assert.AreEqual(ApprovalStatus.Canceled, updatedApproval.Status);
                Assert.AreEqual(comment, updatedApproval.Comments);
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetProfile"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetProfileTest()
        {
            var service = new VstsService();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProfile(null));

            using (ShimsContext.Create())
            {
                var expected = new Profile();

                InitializeConnectionShim(GetProfileHttpClient(expected));

                var actual = await service.GetProfile(this.token);

                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetAccounts"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetAccountsTest()
        {
            var service = new VstsService();

            var memberId = Guid.NewGuid();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.GetAccounts(null, memberId));

            using (ShimsContext.Create())
            {
                var expected = new List<Account>();

                InitializeConnectionShim(GetAccountHttpClient(expected));

                var actual = await service.GetAccounts(this.token, memberId);

                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetApprovals"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        [TestMethod]
        public async Task GetApprovalsTest()
        {
            var service = new VstsService();
            string accountName = "MyAccount";
            string projectName = "MyProject";
            VstsProfile profile = new VstsProfile
            {
                Id = Guid.NewGuid(),
                Token = this.token,
                EmailAddress = "me@email.com"
            };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApprovals(null, projectName, profile));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApprovals(accountName, null, profile));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApprovals(accountName, projectName, null));

            using (ShimsContext.Create())
            {
                var expected = new List<ReleaseApproval>
                {
                    new ReleaseApproval
                    {
                        Id = 1234,
                        ApprovalType = ApprovalType.Undefined
                    }
                };

                IPagedCollection<ReleaseApproval> pagedCollection = new StubIPagedCollection<ReleaseApproval>
                {
                    CountGet = () => expected.Count,
                    ItemGetInt32 = i => expected[i]
                };

                var shimClient = new ShimReleaseHttpClient2
                {
                    GetApprovalsAsync2StringStringNullableOfApprovalStatusIEnumerableOfInt32NullableOfApprovalTypeNullableOfInt32NullableOfInt32NullableOfReleaseQueryOrderNullableOfBooleanObjectCancellationToken
                        = (project, assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top, continuationToken, queryOrder, includeMyGroupApprovals, userState, cancellationToken) =>
                            Task.Run(() => pagedCollection, cancellationToken),
                };

                InitializeConnectionShim(shimClient.Instance);

                var actual = await service.GetApprovals(accountName, projectName, profile);

                Assert.AreEqual(expected.Count, actual.Count);

                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetProjects"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetProjectsTest()
        {
            var service = new VstsService();

            var account = "anaccount";

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetProjects(account, null));

            using (ShimsContext.Create())
            {
                var accounts = new List<Account>
                {
                    new Account(Guid.Empty)
                    {
                        AccountName = "myaccount",
                        AccountUri = new Uri("https://myaccount.visualstudio.com")
                    }
                };

                var expected = new List<TeamProjectReference>();

                var clients = new VssHttpClientBase[]
                {
                    GetAccountHttpClient(accounts), GetProjectHttpClient(expected), GetProfileHttpClient(new Profile())
                };

                InitializeConnectionShim(clients);

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(
                    async () => await service.GetProjects("someaccount", this.token));

                var actual = await service.GetProjects("myaccount", this.token);
                Assert.AreEqual(expected, actual);
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetBuildDefinitionsAsync"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetBuildDefinitionsTest()
        {
            var service = new VstsService();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync(null, "myaccount", this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync("myproject", null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync("myproject", "myaccount", null));

            using (ShimsContext.Create())
            {
                var expected = new List<BuildDefinitionReference>();

                var clients = new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProjectHttpClient(new List<TeamProjectReference> { new TeamProjectReference { Name = "myproject" } }),
                    GetProfileHttpClient(new Profile()),
                    new ShimBuildHttpClient
                    {
                        GetDefinitionsAsyncGuidStringStringStringNullableOfDefinitionQueryOrderNullableOfInt32StringNullableOfDateTimeIEnumerableOfInt32StringNullableOfDateTimeNullableOfDateTimeObjectCancellationToken =
                            (guid, s, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, cancellationToken) => Task.Run(() => expected, cancellationToken)
                    }.Instance
                };

                InitializeConnectionShim(clients);

                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(
                    async () => await service.GetBuildDefinitionsAsync("hisproject", "myaccount", this.token));
                await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(
                    async () => await service.GetBuildDefinitionsAsync("myproject", "hisaccount", this.token));

                var actual = await service.GetBuildDefinitionsAsync("myproject", "myaccount", this.token);
                Assert.AreEqual(expected, actual);
            }
        }

        private static void InitializeConnectionShim(VssHttpClientBase client) => InitializeConnectionShim(new[] { client });

        private static void InitializeConnectionShim(VssHttpClientBase[] clients)
        {
            ShimVssConnection.AllInstances
                    .GetClientServiceImplAsyncTypeGuidFuncOfTypeGuidCancellationTokenTaskOfObjectCancellationToken =
                (connection, type, arg3, arg4, cancellationToken) => Task.Run(
                    () => (object)clients.FirstOrDefault(client => client.GetType() == type), cancellationToken);
        }

        private static ProfileHttpClient GetProfileHttpClient(Profile profile)
        {
            var shimProfileClient = new ShimProfileHttpClient
            {
                GetProfileAsyncProfileQueryContextObjectCancellationToken =
                    (context, objectState, cancelationToken) => Task.Run(() => profile, cancelationToken)
            };

            return shimProfileClient.Instance;
        }

        private static AccountHttpClient GetAccountHttpClient(List<Account> accounts)
        {
            var shimClient = new ShimAccountHttpClient
            {
                GetAccountsByMemberAsyncGuidIEnumerableOfStringObjectCancellationToken =
                    (memberId, filter, state, cancelationToken) => Task.Run(() => accounts, cancelationToken)
            };

            return shimClient.Instance;
        }

        private static ProjectHttpClient GetProjectHttpClient(IEnumerable<TeamProjectReference> projects)
        {
            var shimClient = new ShimProjectHttpClient
            {
                GetProjectsNullableOfProjectStateNullableOfInt32NullableOfInt32Object = (stateFilter, top, skip, userState) => Task.Run(() => projects)
            };

            return shimClient.Instance;
        }
    }
}
