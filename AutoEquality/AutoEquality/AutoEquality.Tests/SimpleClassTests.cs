namespace AutoEquality.Tests
{
    using Ploeh.AutoFixture;
    using Shouldly;
    using Xunit;

    public class SimpleClassTests
    {
        [Fact]
        public void MatchingValuesShouldEqualTrue()
        {
            var value = new Fixture().Create<string>();
            var simpleClass1 = new SinglePropertyClass() { Property = value };
            var simpleClass2 = new SinglePropertyClass() { Property = value };

            var comparer = new AutoEqualityComparer<SinglePropertyClass>();

            var result = comparer.Equals(simpleClass1, simpleClass2);

            result.ShouldBeTrue();
        }

        [Fact]
        public void NonMatchingValuesShouldEqualTrue()
        {
            var value = new Fixture().Create<string>();
            var simpleClass1 = new SinglePropertyClass() { Property = value.ToLower() };
            var simpleClass2 = new SinglePropertyClass() { Property = value.ToUpper() };

            var comparer = new AutoEqualityComparer<SinglePropertyClass>();

            var result = comparer.Equals(simpleClass1, simpleClass2);

            result.ShouldBeFalse();
        }

        internal class SinglePropertyClass
        {
            public string Property { get; set; }
        }
    }
}