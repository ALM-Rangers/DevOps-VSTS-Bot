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
    using System.Collections.Specialized;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dependencies;
    using App_Start;
    using Autofac;
    using DI;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;
    using Moq;
    using TechTalk.SpecFlow;

    [Binding]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Managed by specflow")]
    [SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", Justification = "Managed by specflow")]
    public class ControllerSteps
    {
        private ControllerData data;
        private Mock<IDialogInvoker> mockInvoker = new Mock<IDialogInvoker>();
        private Func<IDialog<object>> invokedMakeRoot;
        private IMessageActivity invokedActivity;
        private Activity activity;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerSteps"/> class.
        /// </summary>
        /// <param name="data">The controller data to be used by the tests.</param>
        public ControllerSteps(ControllerData data)
        {
            this.data = data;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Controller setup needs it")]
        [Given(@"I have a controller")]
        public void GivenIHaveAController()
        {
            IConfigurationProvider configProvider = SetupConfiguration();
            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance<IDialogInvoker>(this.mockInvoker.Object);
            IContainer container = Bootstrap.Build(builder, configProvider, false);
            this.mockInvoker
                .Setup(di => di.SendAsync(It.IsAny<IMessageActivity>(), It.IsAny<Func<IDialog<object>>>()))
                .Callback<IMessageActivity, Func<IDialog<object>>>((toBot, makeRoot) =>
                {
                    this.invokedActivity = toBot;
                    this.invokedMakeRoot = makeRoot;
                })
                .Returns(Task.CompletedTask);

            this.data.HttpConfiguration = new HttpConfiguration();
            WebApiConfig.Register(this.data.HttpConfiguration, container);
            IDependencyScope scope = this.data.HttpConfiguration.DependencyResolver.BeginScope();
            this.data.Controller = (MessagesController)scope.GetService(typeof(MessagesController));
            this.data.Controller.ControllerContext = new HttpControllerContext();
            this.data.Controller.Request = new HttpRequestMessage();
            this.data.Controller.RequestContext = new HttpRequestContext();
        }

        [Given(@"There is a problem invoking the root dialog")]
        public void GivenThereIsAProblemInvokingTheRootDialog()
        {
            this.mockInvoker
                .Setup(di => di.SendAsync(It.IsAny<IMessageActivity>(), It.IsAny<Func<IDialog<object>>>()))
                .Throws<InvalidOperationException>();
        }

        [When(@"I post a message activity to the controller")]
        public void WhenIPostAMessageActivityToTheController()
        {
            this.activity = new Activity();
            this.activity.Type = ActivityTypes.Message;
            this.data.Response = this.data.Controller.Post(this.activity).Result;
        }

        [When(@"I post a non-message activity to the controller")]
        public void WhenIPostANonMessageActivityToTheController()
        {
            this.activity = new Activity();
            this.activity.Type = ActivityTypes.ConversationUpdate;
            this.data.Response = this.data.Controller.Post(this.activity).Result;
        }

        [Then(@"the root dialog is not invoked")]
        public void ThenTheRootDialogIsNotInvoked()
        {
            this.mockInvoker.Verify(di => di.SendAsync(It.IsAny<IMessageActivity>(), It.IsAny<Func<IDialog<object>>>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Then(@"the root dialog is invoked")]
        public void ThenTheRootDialogIsInvoked()
        {
            this.invokedMakeRoot().Should().BeAssignableTo<RootDialog>();
        }

        [Then(@"I get a HTTP (.*) response")]
        public void ThenIGetAHttpResponse(int expectedStatus)
        {
            int status = (int)this.data.Response.StatusCode;
            status.Should().Be(expectedStatus);
        }

        [Then(@"the activity is passed to the dialog")]
        public void ThenTheActivityIsPassedToTheDialog()
        {
            this.invokedActivity.Should().BeSameAs(this.activity);
        }

        private static IConfigurationProvider SetupConfiguration()
        {
            NameValueCollection config = new NameValueCollection();
            config.Add(ConfigurationSettingName.MicrosoftApplicationId, Guid.NewGuid().ToString());
            config.Add(ConfigurationSettingName.MicrosoftApplicationPassword, Guid.NewGuid().ToString());
            config.Add(ConfigurationSettingName.ApplicationId, Guid.NewGuid().ToString());
            config.Add(ConfigurationSettingName.ApplicationSecret, Guid.NewGuid().ToString());
            config.Add(ConfigurationSettingName.AuthorizeUrl, "http://localhost/");

            Mock<IConfigurationProvider> mockConfigProvider = new Mock<IConfigurationProvider>();
            mockConfigProvider
                .Setup(cp => cp.GetValue(It.IsAny<string>()))
                .Returns((string name) => config[name]);
            return mockConfigProvider.Object;
        }
    }
}
