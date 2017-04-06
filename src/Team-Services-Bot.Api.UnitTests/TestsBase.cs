//———————————————————————————————
// <copyright file=”name of this file, i.e. TestsBase.cs“>
// Licensed under the MIT License. See License.txt in the project root for license information.
// </copyright>
// <summary>
// Base class for all tests that require a fixture.
// </summary>
//———————————————————————————————

namespace Vsar.TSBot.UnitTests
{
    public abstract class TestsBase<T> where T : class
    {
        protected TestsBase(T fixture)
        {
            Fixture = fixture;
        }

        public T Fixture { get; private set; }
    }
}