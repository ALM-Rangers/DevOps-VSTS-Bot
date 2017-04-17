//———————————————————————————————
// <copyright file=”name of this file, i.e. RootDialogTests.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the RootDialog.
// </summary>
//———————————————————————————————

using System;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extras.AttributeMetadata;
using FluentAssertions;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vsar.TSBot.Dialogs;

namespace Vsar.TSBot.UnitTests
{
    [TestClass]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        public RootDialogTests() : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task EchoDialog()
        {
            var toBot = Fixture.CreateMessage();
            toBot.From.Id = Guid.NewGuid().ToString();
            toBot.Text = "Test";

            var telemetryClient = new TelemetryClient();

            var builder = Fixture.Build();
            builder
                .RegisterType<EchoDialog>()
                .As<IDialog<object>>();

            using (var container = builder.Build())
            {
                var root = new RootDialog(container, telemetryClient);

                var toUser = await Fixture.GetResponse(container, root, toBot);

                toUser.Text.Should().Contain("4").And.Contain("Test");
            }
        }
    }
}