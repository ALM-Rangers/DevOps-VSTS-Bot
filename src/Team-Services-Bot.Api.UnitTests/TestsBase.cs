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