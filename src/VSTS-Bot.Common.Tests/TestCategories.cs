// ———————————————————————————————
// <copyright file="TestCategories.cs"  >
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Defines the test categories.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.Common.Tests
#pragma warning restore SA1640 // File header must have valid company text
{
    /// <summary>
    /// Defines the test categories
    /// </summary>
    public static class TestCategories
    {
        /// <summary>
        /// The test category name for behavioural tests.
        /// </summary>
        public const string Behavioural = "Behavioural";

        /// <summary>
        /// The test category name for integration tests.
        /// </summary>
        public const string Integration = "Integration";

        /// <summary>
        /// The test category name for unit tests.
        /// </summary>
        public const string Unit = "Unit";
    }
}
