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
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac;
    using Cards;
    using Dialogs;
    using FluentAssertions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;

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
        /// Tests the first time welcome message.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task Welcome_First_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var builder = new ContainerBuilder();
            var container = this.Fixture.Build(builder);

            // First trigger the welcome message.
            var toUser = await this.Fixture.GetResponse(container, this.Fixture.RootDialog, toBot);

            toUser.Text.Should().Be($"Welcome {toBot.From.Name}. I see this is the first team we speak.");
        }

        /// <summary>
        /// Tests the first time welcome message.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
        public async Task Welcome_Second_Time()
        {
            var toBot = this.Fixture.CreateMessage();
            toBot.Text = "Hi";

            var account = "anaccount";
            var profile = new VstsProfile();
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

        /// <summary>
        /// Tests the first time welcome message.
        /// </summary>
        /// <returns>Nothing.</returns>
        [TestMethod]
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