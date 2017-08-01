// ———————————————————————————————
// <copyright file="TestsBase.cs">
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Base class for all tests that require a fixture.
// </summary>
// ———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Parent class for all dialog tests.
    /// </summary>
    /// <typeparam name="T">A fixture.</typeparam>
    [ExcludeFromCodeCoverage]
    public abstract class TestsBase<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestsBase{T}"/> class.
        /// </summary>
        /// <param name="fixture">A fixture.</param>
        protected TestsBase(T fixture)
        {
            this.Fixture = fixture;
        }

        /// <summary>
        /// Gets a Fixture.
        /// </summary>
        public T Fixture { get; }
    }
}