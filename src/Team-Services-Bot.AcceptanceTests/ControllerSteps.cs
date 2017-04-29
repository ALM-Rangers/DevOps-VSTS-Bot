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
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dependencies;
    using Autofac;
    using DI;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Moq;
    using TechTalk.SpecFlow;

    [Binding]
    public class ControllerSteps
    {
        private MessagesController controller;
        private HttpResponseMessage response;
        private Mock<IDialogInvoker> mockInvoker = new Mock<IDialogInvoker>();
        private Func<IDialog<object>> invokedMakeRoot;
        private IMessageActivity invokedActivity;
        private Activity activity;

        [Given(@"I have a controller")]
        public void GivenIHaveAController()
        {
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance<IDialogInvoker>(this.mockInvoker.Object);
            this.mockInvoker
                .Setup(di => di.SendAsync(It.IsAny<IMessageActivity>(), It.IsAny<Func<IDialog<object>>>(), It.IsAny<CancellationToken>()))
                .Callback<IMessageActivity, Func<IDialog<object>>, CancellationToken>((toBot, makeRoot, token) =>
                {
                    this.invokedActivity = toBot;
                    this.invokedMakeRoot = makeRoot;
                })
                .Returns(Task.CompletedTask);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config, builder);
            IDependencyScope scope = config.DependencyResolver.BeginScope();
            this.controller = (MessagesController)scope.GetService(typeof(MessagesController));
            this.controller.ControllerContext = new HttpControllerContext();
            this.controller.Request = new HttpRequestMessage();
            this.controller.RequestContext = new HttpRequestContext();
        }

        [Given(@"There is a problem invoking the root dialog")]
        public void GivenThereIsAProblemInvokingTheRootDialog()
        {
            this.mockInvoker
                .Setup(di => di.SendAsync(It.IsAny<IMessageActivity>(), It.IsAny<Func<IDialog<object>>>(), It.IsAny<CancellationToken>()))
                .Throws<InvalidOperationException>();
        }

        [When(@"I post a message activity to the controller")]
        public void WhenIPostAMessageActivityToTheController()
        {
            this.activity = new Activity();
            this.activity.Type = ActivityTypes.Message;
            this.response = this.controller.Post(this.activity).Result;
        }

        [Then(@"the root dialog is invoked")]
        public void ThenTheRootDialogIsInvoked()
        {
            this.invokedMakeRoot().Should().BeAssignableTo<RootDialog>();
        }

        [Then(@"I get a HTTP (.*) response")]
        public void ThenIGetAHTTPResponse(int expectedStatus)
        {
            int status = (int)this.response.StatusCode;
            status.Should().Be(expectedStatus);
        }

        [Then(@"the activity is passed to the dialog")]
        public void ThenTheActivityIsPassedToTheDialog()
        {
            this.invokedActivity.Should().BeSameAs(this.activity);
        }
    }
}
