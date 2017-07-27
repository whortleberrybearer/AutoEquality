namespace AutoEquality.Tests
{
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class SimpleClassTests
    {
        [Theory]
        [InlineAutoData]
        public void IgnoredMultipleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3 };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3 };

            sut.Ignore(a => a.Property2).Ignore(a => a.Property1);

            var result = sut.Equals(simpleClass1, simpleClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void IgnoredSingleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToLower(), Property3 = value3 };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToUpper(), Property3 = value3 };

            sut.Ignore(a => a.Property2);

            var result = sut.Equals(simpleClass1, simpleClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void MatchingValuesShouldEqualTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

            var result = sut.Equals(simpleClass1, simpleClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingValuesShouldEqualTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

            var result = sut.Equals(simpleClass1, simpleClass2);

            result.ShouldBeFalse();
        }

        public class MultiPropertyClass
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }

            public string Property3 { get; set; }
        }
    }
}