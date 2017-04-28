// ———————————————————————————————
// <copyright file="RootDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the RootDialog.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using System.Web.Http;
    using Autofac;
    using Autofac.Integration.WebApi;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Contains Test methods.
    /// </summary>
    [TestClass]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootDialogTests"/> class.
        /// </summary>
        public RootDialogTests()
            : base(new DialogFixture())
        {
        }

        /// <summary>
        /// Temporary
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task EchoDialog()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "Test";

            var builder = this.Fixture.Build();
            builder.RegisterType<TelemetryClient>();
            builder
                .RegisterType<EchoDialog>()
                .As<IDialog<object>>();

            var container = builder.Build();
            GlobalConfiguration.Configure(config => config.DependencyResolver = new AutofacWebApiDependencyResolver(container));

            var root = new RootDialog();

            var toUser = await this.Fixture.GetResponse(container, root, toBot);

            toUser.Text.Should().Contain("4").And.Contain("Test");
        }
    }
}