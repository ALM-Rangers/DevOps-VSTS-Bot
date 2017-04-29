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
    using System.Web.Http;
    using System.Web.Http.Dependencies;
    using Microsoft.Bot.Connector;
    using TechTalk.SpecFlow;

    [Binding]
    public class ControllerSteps
    {
        private MessagesController controller;

        [Given(@"I have a controller")]
        public void GivenIHaveAController()
        {
            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            IDependencyScope scope = config.DependencyResolver.BeginScope();
            this.controller = (MessagesController)scope.GetService(typeof(MessagesController));
        }

        [When(@"I post a message activity to the controller")]
        public void WhenIPostAMessageActivityToTheController()
        {
            Activity activity = new Activity();
            this.controller.Post(activity).Wait();
        }

        [Then(@"the root dialog is invoked")]
        public void ThenTheRootDialogIsInvoked()
        {
            ScenarioContext.Current.Pending();
        }
    }
}
