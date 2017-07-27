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
            var value1 = new Fixture().Create<string>();
            var value2 = new Fixture().Create<string>();
            var value3 = new Fixture().Create<string>();
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

            var comparer = new AutoEqualityComparer<MultiPropertyClass>();

            var result = comparer.Equals(simpleClass1, simpleClass2);

            result.ShouldBeTrue();
        }

        [Fact]
        public void NonMatchingValuesShouldEqualTrue()
        {
            var value1 = new Fixture().Create<string>();
            var value2 = new Fixture().Create<string>();
            var value3 = new Fixture().Create<string>();
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };

            var comparer = new AutoEqualityComparer<MultiPropertyClass>();

            var result = comparer.Equals(simpleClass1, simpleClass2);

            result.ShouldBeFalse();
        }

        internal class MultiPropertyClass
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }

            public string Property3 { get; set; }
        }
    }
}