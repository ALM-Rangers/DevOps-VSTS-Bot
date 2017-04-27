// ———————————————————————————————
// <copyright file="ControllerSteps.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the specflow steps to perform an echo.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.AcceptanceTests
{
    using System;
    using TechTalk.SpecFlow;

    [Binding]
    public class ControllerSteps
    {
        private MessagesController controller;

        [Given(@"I have a controller")]
        public void GivenIHaveAController()
        {
            ScenarioContext.Current.Pending();
        }
        
        [When(@"I post a message activity to the controller")]
        public void WhenIPostAMessageActivityToTheController()
        {
            ScenarioContext.Current.Pending();
        }
        
        [Then(@"the root dialog is invoked")]
        public void ThenTheRootDialogIsInvoked()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
