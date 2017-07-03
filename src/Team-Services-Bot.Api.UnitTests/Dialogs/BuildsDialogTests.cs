// ———————————————————————————————
// <copyright file="BuildsDialogTests.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Contains the tests for the BuildsDialog.
// </summary>
// ———————————————————————————————
namespace Vsar.TSBot.UnitTests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Common.Tests;
    using Dialogs;
    using Microsoft.Bot.Connector;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

    [TestClass]
    [TestCategory(TestCategories.Unit)]
    [ExcludeFromCodeCoverage]
    public class BuildsDialogTests : TestsBase<DialogFixture>
    {
        public BuildsDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        public async Task Constructor_Empty_VstsService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new BuildsDialog(null));

            await Task.CompletedTask;
        }

        [TestMethod]
        public async Task Start()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<BuildsDialog>(this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await target.StartAsync(this.Fixture.DialogContext.Object);

            this.Fixture.DialogContext.Verify(c => c.Wait<IMessageActivity>(target.BuildsAsync));
        }

        [TestMethod]
        public async Task Builds_Missing_Context()
        {
            var target = new BuildsDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.BuildsAsync(null, null));
        }

        [TestMethod]
        public async Task Builds_Missing_Awaitable()
        {
            var target = new BuildsDialog(this.Fixture.VstsService.Object);

            await Assert.ThrowsExceptionAsync<ArgumentNullException>(async () => await target.BuildsAsync(this.Fixture.DialogContext.Object, null));
        }

        [TestMethod]
        public async Task Builds_No_Text()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = null;

            var mocked = new Mock<BuildsDialog>(this.Fixture.VstsService.Object) { CallBase = true };
            var target = mocked.Object;

            await Task.CompletedTask;
        }
    }
}