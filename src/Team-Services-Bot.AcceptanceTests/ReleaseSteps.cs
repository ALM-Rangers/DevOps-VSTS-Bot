// ———————————————————————————————
// <copyright file="ReleaseSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Represents the Steps for a release.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.AcceptanceTests
{
    using System.Threading.Tasks;
    using TechTalk.SpecFlow;

    [Binding]
    public class ReleaseSteps
    {
        [Given(@"I started release '(\d*)' on '(.*)'")]
        public async Task GivenIStartedOn(int definitionId, string teamProject)
        {
            var service = new VstsService();

            await service.ReleaseQueue(Config.Account, teamProject, Config.Token, definitionId);
        }
    }
}