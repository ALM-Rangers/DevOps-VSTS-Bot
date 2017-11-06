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
    using FluentAssertions;
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
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
    using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Fakes;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.VisualStudio.Services.WebApi.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Profile = TSBot.Profile;

    /// <summary>
    /// Unit tests for <see cref="VstsService"/>.
    /// </summary>
    [TestClass]
    [TestCategory("Unit")]
    public class VstsServiceTest
    {
        private readonly OAuthToken token = new OAuthToken { AccessToken = @"x25onorum4neacdjmvzvaxjeosik7qxo7fbnn6lebefeday7fxmq" };

        /// <summary>
        /// Tests <see cref="VstsService.ChangeApprovalStatus"/>
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        [TestMethod]
        public async Task ChangeApprovalStatusTest()
        {
            var account = "MyAccount";
            var project = "MyProject";
            const int id = 1234;
            var comment = "My comment";
            var status = ApprovalStatus.Undefined;

            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Token = this.token,
                DisplayName = "me"
            };

            var service = new VstsService();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(null, project, profile, id, status, comment));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(account, null, profile, id, status, comment));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.ChangeApprovalStatus(account, project, null, id, status, comment));

            using (ShimsContext.Create())
            {
                ReleaseApproval updatedApproval = null;

                InitializeConnectionShim(new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProfileHttpClient(new Microsoft.VisualStudio.Services.Profile.Profile()),
                    new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                    {
                        GetApprovalAsyncStringInt32NullableOfBooleanObjectCancellationToken = (p, i, includeHistory, userState, cancellationToken) => Task.Run(
                            () => new ReleaseApproval { Id = i },
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
                });

                await service.ChangeApprovalStatus(account, project, profile, 4, ApprovalStatus.Canceled, comment);

                Assert.IsNotNull(updatedApproval);
                Assert.AreEqual(4, updatedApproval.Id);
                Assert.AreEqual(ApprovalStatus.Canceled, updatedApproval.Status);
                Assert.AreEqual(comment, updatedApproval.Comments);
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.CreateReleaseAsync"/> method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing asynchronous unit test.</returns>
        [TestMethod]
        public async Task CreateReleaseAsyncTest()
        {
            var accountName = "myaccount";
            var projectName = "myproject";
            var service = new VstsService();
            int id = 1;

            var builds = new List<Build> { new Build { Id = 12345, LastChangedDate = DateTime.Now } };

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.CreateReleaseAsync(null, projectName, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.CreateReleaseAsync(accountName, null, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.CreateReleaseAsync(accountName, projectName, 0, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.CreateReleaseAsync(accountName, projectName, id, null));

            using (ShimsContext.Create())
            {
                var shimBuildHttpClient = new ShimBuildHttpClient();

                shimBuildHttpClient.SendAsyncOf1HttpMethodGuidObjectApiResourceVersionHttpContentIEnumerableOfKeyValuePairOfStringStringObjectCancellationTokenFuncOfHttpResponseMessageCancellationTokenTaskOfM0<IPagedList<Build>>((method, guid, arg3, apiResourceVersion, content, queryParams, arg7, cancellationToken, arg9) =>
                    Task.Run(
                        () => new PagedList<Build>(builds, string.Empty) as IPagedList<Build>,
                        cancellationToken));

                InitializeConnectionShim(new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProfileHttpClient(new Microsoft.VisualStudio.Services.Profile.Profile()),
                    new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                    {
                        GetReleaseDefinitionAsyncStringInt32IEnumerableOfStringObjectCancellationToken = (project, definitionId, filters, userState, cancellationToken) => Task.Run(
                            () =>
                            {
                                return new ReleaseDefinition()
                                {
                                    Artifacts = new List<Artifact>
                                    {
                                        new Artifact
                                        {
                                            IsPrimary = true,
                                            Alias = "mybuildartifcat",
                                            DefinitionReference = new Dictionary<string, ArtifactSourceReference>
                                            {
                                                { "definition", new ArtifactSourceReference { Id = "1234" } }
                                            },
                                            Type = ArtifactTypes.BuildArtifactType
                                        }
                                    }
                                };
                            }),
                        CreateReleaseAsyncReleaseStartMetadataStringObjectCancellationToken = (startMetadata, project, userState, cancellationToken) => Task.Run(
                            () =>
                            {
                                Assert.AreEqual(projectName, project);
                                return new Release();
                            },
                            cancellationToken)
                    },
                    shimBuildHttpClient.Instance
                });

                await service.CreateReleaseAsync(accountName, projectName, id, this.token);
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
            var accountName = "MyAccount";
            var projectName = "MyProject";
            var profile = new Profile
            {
                Id = Guid.NewGuid(),
                Token = this.token,
                DisplayName = "me"
            };
            var service = new VstsService();

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

                InitializeConnectionShim(new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProfileHttpClient(new Microsoft.VisualStudio.Services.Profile.Profile()),
                    new ShimReleaseHttpClient2
                    {
                        GetApprovalsAsync2StringStringNullableOfApprovalStatusIEnumerableOfInt32NullableOfApprovalTypeNullableOfInt32NullableOfInt32NullableOfReleaseQueryOrderNullableOfBooleanObjectCancellationToken
                            = (project, assignedToFilter, statusFilter, releaseIdsFilter, typeFilter, top, continuationToken, queryOrder, includeMyGroupApprovals, userState, cancellationToken) =>
                                Task.Run(
                                    () => (IPagedCollection<ReleaseApproval>)new StubIPagedCollection<ReleaseApproval>
                                    {
                                        CountGet = () => expected.Count,
                                        ItemGetInt32 = i => expected[i]
                                    },
                                    cancellationToken)
                    }.Instance
                });

                var actual = await service.GetApprovals(accountName, projectName, profile);

                Assert.AreEqual(expected.Count, actual.Count);

                for (var i = 0; i < actual.Count; i++)
                {
                    Assert.AreEqual(expected[i], actual[i]);
                }
            }
        }

        /// <summary>
        /// Tests <see cref="VstsService.GetApproval"/> method.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing asynchronous unit test.</returns>
        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetApprovalTest()
        {
            var accountName = "myaccount";
            var projectName = "myproject";
            var service = new VstsService();
            int id = 1;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApproval(null, projectName, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApproval(accountName, null, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.GetApproval(accountName, projectName, 0, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetApproval(accountName, projectName, id, null));

            using (ShimsContext.Create())
            {
                var expected = new ReleaseApproval
                {
                    Id = 1234,
                    ApprovalType = ApprovalType.Undefined
                };

                InitializeConnectionShim(new VssHttpClientBase[]
                {
                    GetAccountHttpClient(new List<Account>
                    {
                        new Account(Guid.Empty)
                        {
                            AccountName = "myaccount",
                            AccountUri = new Uri("https://myaccount.visualstudio.com")
                        }
                    }),
                    GetProfileHttpClient(new Microsoft.VisualStudio.Services.Profile.Profile()),
                    new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                    {
                        GetApprovalAsyncStringInt32NullableOfBooleanObjectCancellationToken
                            = (project, approvalId, includeHistory, objectState, cancellationToken) => Task.Run(
                                () =>
                                {
                                    expected.Id = approvalId;
                                    return expected;
                                },
                                cancellationToken)
                    }.Instance
                });

                ReleaseApproval actual = await service.GetApproval(accountName, projectName, id, this.token);

                Assert.IsNotNull(actual);
                Assert.AreEqual(expected, actual);
                Assert.AreEqual(id, actual.Id);
            }
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetBuildTest()
        {
            var accountName = "myaccount";
            var teamProjectName = "myproject";
            var service = new VstsService();
            var id = 1;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildAsync(null, teamProjectName, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildAsync(accountName, null, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.GetBuildAsync(accountName, teamProjectName, 0, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildAsync(accountName, teamProjectName, 1, null));

            using (ShimsContext.Create())
            {
                var client = new ShimBuildHttpClientBase(new ShimBuildHttpClient());

                InitializeConnectionShim(client.Instance);

                client.GetBuildAsyncStringInt32StringObjectCancellationToken = (teamProject, buildId, arg3, arg4, cancellationToken) =>
                    Task.Run(
                        () =>
                        {
                            teamProject.Should().Be(teamProjectName);
                            buildId.Should().Be(id);

                            return new Build();
                        },
                        cancellationToken);

                await service.GetBuildAsync(accountName, teamProjectName, id, this.token);
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

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync(null, "myproject", this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync("myaccount", null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync("myaccount", "myproject", null));

            using (ShimsContext.Create())
            {
                var expected = new List<BuildDefinitionReference>();

                var clients = new VssHttpClientBase[]
                {
                    new ShimBuildHttpClient
                    {
                        GetDefinitionsAsyncStringStringStringStringNullableOfDefinitionQueryOrderNullableOfInt32StringNullableOfDateTimeIEnumerableOfInt32StringNullableOfDateTimeNullableOfDateTimeObjectCancellationToken =
                            (s, s1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, cancellationToken) => Task.Run(() => expected, cancellationToken)
                    }.Instance
                };

                InitializeConnectionShim(clients);

                IEnumerable<BuildDefinitionReference> actual = await service.GetBuildDefinitionsAsync("myaccount", "myproject", this.token);

                expected.ShouldAllBeEquivalentTo(actual);
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
                var expected = new Microsoft.VisualStudio.Services.Profile.Profile();

                InitializeConnectionShim(GetProfileHttpClient(expected));

                Microsoft.VisualStudio.Services.Profile.Profile actual = await service.GetProfile(this.token);

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

            Guid memberId = Guid.NewGuid();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(() => service.GetAccounts(null, memberId));

            using (ShimsContext.Create())
            {
                var expected = new List<Account>();

                InitializeConnectionShim(GetAccountHttpClient(expected));

                IList<Account> actual = await service.GetAccounts(this.token, memberId);

                Assert.AreEqual(expected, actual);
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
            var account = "anaccount";
            var service = new VstsService();

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

                var expected = new List<TeamProjectReference>
                {
                    new TeamProjectReference
                    {
                        Id = Guid.NewGuid(),
                        Name = "My Project",
                        Url = "https://myaccount.visualstudio.com/my%20project"
                    }
                };

                var clients = new VssHttpClientBase[]
                {
                    GetAccountHttpClient(accounts), GetProjectHttpClient(expected), GetProfileHttpClient(new Microsoft.VisualStudio.Services.Profile.Profile())
                };

                InitializeConnectionShim(clients);

                ////  await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.GetProjects("someaccount", this.token));
                IEnumerable<TeamProjectReference> actual = await service.GetProjects(accounts[0].AccountName, this.token);

                expected.ShouldAllBeEquivalentTo(actual);
            }
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetReleaseAsyncTest()
        {
            var accountName = "myaccount";
            var projectName = "myproject";
            var release = new Release();
            var releaseId = 99;
            var service = new VstsService();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetReleaseAsync(null, projectName, releaseId, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetReleaseAsync(accountName, null, releaseId, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.GetReleaseAsync(accountName, projectName, -10, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetReleaseAsync(accountName, projectName, releaseId, null));

            using (ShimsContext.Create())
            {
                new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                {
                    GetReleaseAsyncStringInt32NullableOfBooleanIEnumerableOfStringObjectCancellationToken =
                        (teamProject, id, arg3, arg4, arg5, cancellationToken) =>
                            Task.Run(
                                () =>
                                {
                                    teamProject.Should().Be(projectName);
                                    id.Should().Be(releaseId);

                                    return release;
                                },
                                cancellationToken)
                };
            }
        }

        [TestMethod]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Test method shouldn't be a property. Test method name corresponds to method under test.")]
        public async Task GetReleaseDefinitionsAsyncTest()
        {
            var accountName = "myaccount";
            var projectName = "myproject";
            var service = new VstsService();

            var expected = new List<ReleaseDefinition>();

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync(null, projectName, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync(accountName, null, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.GetBuildDefinitionsAsync(accountName, projectName, null));

            using (ShimsContext.Create())
            {
                var client = new ShimReleaseHttpClientBase(new ShimReleaseHttpClient2())
                {
                    GetReleaseDefinitionsAsyncStringStringNullableOfReleaseDefinitionExpandsStringStringNullableOfInt32StringNullableOfReleaseDefinitionQueryOrderStringNullableOfBooleanIEnumerableOfStringIEnumerableOfStringIEnumerableOfStringObjectCancellationToken
                        = (teamProject, s1, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, cancellationToken) =>
                                Task.Run(
                                    () =>
                                    {
                                        teamProject.Should().Be(projectName);

                                        return expected;
                                    },
                                    cancellationToken)
                };

                InitializeConnectionShim(client);

                var actual = await service.GetReleaseDefinitionsAsync(accountName, projectName, this.token);

                actual.ShouldBeEquivalentTo(expected);
            }
        }

        [TestMethod]
        public async Task QueueBuildAsyncTest()
        {
            var accountName = "myaccount";
            var teamProjectName = "myproject";
            var service = new VstsService();
            var id = 1;

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.QueueBuildAsync(null, teamProjectName, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.QueueBuildAsync(accountName, null, id, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(async () => await service.QueueBuildAsync(accountName, teamProjectName, 0, this.token));
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await service.QueueBuildAsync(accountName, teamProjectName, id, null));

            using (ShimsContext.Create())
            {
                var client = new ShimBuildHttpClientBase(new ShimBuildHttpClient());

                InitializeConnectionShim(client.Instance);

                client.QueueBuildAsyncBuildStringNullableOfBooleanStringObjectCancellationToken =
                    (build, teamProject, arg3, arg4, arg5, cancellationToken) =>
                        Task.Run(
                            () =>
                            {
                                build.Definition.Id.Should().Be(id);
                                teamProject.Should().Be(teamProjectName);

                                return new Build();
                            },
                            cancellationToken);

                await service.QueueBuildAsync(accountName, teamProjectName, id, this.token);
            }
        }

        private static void InitializeConnectionShim(VssHttpClientBase client)
        {
            InitializeConnectionShim(new[] { client });
        }

        private static void InitializeConnectionShim(VssHttpClientBase[] clients)
        {
            ShimVssConnection.AllInstances
                    .GetClientServiceImplAsyncTypeGuidFuncOfTypeGuidCancellationTokenTaskOfObjectCancellationToken =
                (connection, type, arg3, arg4, cancellationToken) => Task.Run(
                    () => (object)clients.FirstOrDefault(client => client.GetType() == type), cancellationToken);
        }

        private static ProfileHttpClient GetProfileHttpClient(Microsoft.VisualStudio.Services.Profile.Profile profile)
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
                GetProjectsNullableOfProjectStateNullableOfInt32NullableOfInt32ObjectString = (stateFilter, top, skip, userState, continuationToken) => Task.Run(() => new PagedList<TeamProjectReference>(projects, continuationToken) as IPagedList<TeamProjectReference>)
            };

            return shimClient.Instance;
        }
    }
}
