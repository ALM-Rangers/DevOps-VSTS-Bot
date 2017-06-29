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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using TechTalk.SpecFlow;

    [Binding]
    public class ReleaseSteps
    {
        [Given(@"I started release '(\d*)' on '(.*)'")]
        public void GivenIStartedOn(int definitionId, KeyValuePair<string, string> teamProject)
        {
            var service = new VstsService();
            service.CreateReleaseAsync(Config.Account, teamProject.Value, definitionId, Config.Token).Wait();

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }
}