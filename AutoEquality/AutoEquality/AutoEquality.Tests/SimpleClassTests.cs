namespace AutoEquality.Tests
{
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class SimpleClassTests
    {
        public class IngoreTests
        {
            [Theory]
            [InlineAutoData]
            public void MultipleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3 };

                sut.IncludeAll().Ignore(a => a.Property2).Ignore(a => a.Property1);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void SingleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToUpper(), Property3 = value3 };

                sut.IncludeAll().Ignore(a => a.Property2);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }

        public class DefaultConstructionTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                var result = sut.Equals(simpleClass1, simpleClass2);

                // The default construction does not setup anything, so a comparison of not properties should be true.
                result.ShouldBeTrue();
            }
        }

        public class IncludeAllTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

                sut.IncludeAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeFalse(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                sut.IncludeAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeFalse();
            }
        }

        public class IncludeTests
        {
            [Theory]
            [InlineAutoData]
            public void MultipleValueShouldBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToUpper(), Property3 = value3 };

                sut.Include(a => a.Property3).Include(a => a.Property1);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void SingleValueShouldBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2, Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2, Property3 = value3.ToUpper() };

                sut.Include(a => a.Property2);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }

        public class IgnoreAllTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

                sut.IgnoreAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                sut.IgnoreAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }

        public class MultiPropertyClass
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }

            public string Property3 { get; set; }
        }
    }
}