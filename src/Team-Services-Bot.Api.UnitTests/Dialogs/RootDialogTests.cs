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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Cards;
    using Common.Tests;
    using FluentAssertions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class RootDialogTests : TestsBase<DialogFixture>
    {
        public RootDialogTests()
            : base(new DialogFixture())
        {
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Welcome_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var builder = new ContainerBuilder();
            var container = this.Fixture.Build(builder);

            // First trigger the welcome message.
            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            toUser.Text.Should().Be($"Welcome {toBot.From.Name}. This is the first time we talk.");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Welcome_Second_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var account = "anaccount";

            var profile = this.Fixture.CreateProfile();
            IList<VstsProfile> profiles = new List<VstsProfile> { profile };
            var teamProject = "TeamProject1";

            var builder = new ContainerBuilder();
            var container = this.Fixture.Build(builder);

            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Account", out account))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profile", out profile))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("Profiles", out profiles))
                .Returns(true);
            this.Fixture.UserData
                .Setup(ud => ud.TryGetValue("TeamProject", out teamProject))
                .Returns(true);

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            toUser.Text.Should().Be($"Welcome back {toBot.From.Name}. I have connected you to Account '{account}', Team Project '{teamProject}'.");
        }

        [TestMethod]
        [TestCategory(TestCategories.Unit)]
        public async Task Show_Options()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var builder = new ContainerBuilder();
            var container = this.Fixture.Build(builder);

            this.Fixture.RootDialog.Initialized = true;

            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            var attachment = toUser.Attachments.FirstOrDefault();
            attachment.Should().NotBeNull();

            var card = attachment.Content;
            card.Should().BeOfType<MainOptionsCard>();
        }
    }
}