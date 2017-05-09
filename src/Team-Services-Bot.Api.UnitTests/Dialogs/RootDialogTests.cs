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
    }
}