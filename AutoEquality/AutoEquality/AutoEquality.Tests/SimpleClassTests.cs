namespace AutoEquality.Tests
{
    using System;
    using System.Linq.Expressions;
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class SimpleClassTests
    {
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
            public void NonMatchingTypeShouldBeFalse(MultiPropertyClass simpleClass1, DeepClass1 simpleClass2, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeFalse();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingTypesShouldBeFalse(MultiPropertyClass simpleClass1, DeepClass1 simpleClass2, AutoEqualityComparer<ArrayClass> sut)
            {
                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeFalse();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeFalse(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                var result = sut.Equals(simpleClass1, simpleClass2);

                // The default construction sets up all properties, so a non match will fail.
                result.ShouldBeFalse();
            }

            [Theory]
            [InlineAutoData]
            public void NullValuesShouldBeFalse(AutoEqualityComparer<object> sut)
            {
                var result = sut.Equals(null, null);

                result.ShouldBeFalse();
            }
        }

        public class WithAllTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

                sut.WithAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeFalse(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                sut.WithAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeFalse();
            }
        }

        public class WithoutAllTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };

                sut.WithoutAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingValuesShouldBeTrue(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3.ToUpper() };

                sut.WithoutAll();

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }

        public class WithoutTests
        {
            [Theory]
            [InlineAutoData]
            public void MultipleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2.ToUpper(), Property3 = value3 };

                sut.Without(a => a.Property2);
                sut.Without(a => a.Property1);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NullPropertyShouldThrowArgumentNullException(AutoEqualityComparer<MultiPropertyClass> sut)
            {
                Should.Throw<ArgumentNullException>(() => sut.With((Expression<Func<MultiPropertyClass, object>>)null));
            }

            [Theory]
            [InlineAutoData]
            public void SingleValueShouldNotBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToUpper(), Property3 = value3 };

                sut.Without(a => a.Property2);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }

        public class WithTests
        {
            [Theory]
            [InlineAutoData]
            public void MultipleValueShouldBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToLower(), Property3 = value3 };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = value2.ToUpper(), Property3 = value3 };

                sut.WithoutAll();
                sut.With(a => a.Property3);
                sut.With(a => a.Property1);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NullPropertyShouldThrowArgumentNullException(AutoEqualityComparer<MultiPropertyClass> sut)
            {
                Should.Throw<ArgumentNullException>(() => sut.With((Expression<Func<MultiPropertyClass, object>>)null));
            }

            [Theory]
            [InlineAutoData]
            public void SingleValueShouldBeCompared(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
            {
                var simpleClass1 = new MultiPropertyClass() { Property1 = value1.ToLower(), Property2 = value2, Property3 = value3.ToLower() };
                var simpleClass2 = new MultiPropertyClass() { Property1 = value1.ToUpper(), Property2 = value2, Property3 = value3.ToUpper() };

                sut.WithoutAll();
                sut.With(a => a.Property2);

                var result = sut.Equals(simpleClass1, simpleClass2);

                result.ShouldBeTrue();
            }
        }
    }
}