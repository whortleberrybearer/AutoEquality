namespace AutoEquality.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class EnumerableTests
    {
        // TODO: Need to test for non generic enumerable.  Also check for multiple generic arguements (dictionary?)

        [Theory]
        [InlineAutoData]
        public void DifferentLengthsShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
        {
            // Skipping and reversing the list means the start elements match, the missing element is the one at the end.
            var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a).Reverse() };
            var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a).Skip(1).Reverse() };

            var result = sut.Equals(enumerableClass1, enumerableClass2);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineAutoData]
        public void MatchingEnumerableValuesShouldBeTrue(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
        {
            var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a) };
            var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a) };

            var result = sut.Equals(enumerableClass1, enumerableClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void MatchingMultiPropertyEnumerableValuesShouldBeTrue(string value1, string value2, string value3, string value4, string value5, string value6, AutoEqualityComparer<EnumerableMultiPropertyClass> sut)
        {
            var multiPropertyEnumerableClass1 = new EnumerableMultiPropertyClass()
            {
                Enumerable = new MultiPropertyClass[]
                {
                    new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 },
                    new MultiPropertyClass() { Property1 = value4, Property2 = value5, Property3 = value6 }
                }
            };
            var multiPropertyEnumerableClass2 = new EnumerableMultiPropertyClass()
            {
                Enumerable = new MultiPropertyClass[]
                {
                    new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 },
                    new MultiPropertyClass() { Property1 = value4, Property2 = value5, Property3 = value6 }
                }
            };

            var result = sut.Equals(multiPropertyEnumerableClass1, multiPropertyEnumerableClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingEnumerableValuesShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
        {
            var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a.ToLower()) };
            var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a.ToUpper()) };

            var result = sut.Equals(enumerableClass1, enumerableClass2);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingMultiPropertyEnumerableValuesShouldBeFalse(string value1, string value2, string value3, string value4, string value5, string value6, AutoEqualityComparer<EnumerableMultiPropertyClass> sut)
        {
            var multiPropertyEnumerableClass1 = new EnumerableMultiPropertyClass()
            {
                Enumerable = new MultiPropertyClass[]
                {
                    new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 },
                    new MultiPropertyClass() { Property1 = value4, Property2 = value5.ToLower(), Property3 = value6 }
                }
            };
            var multiPropertyEnumerableClass2 = new EnumerableMultiPropertyClass()
            {
                Enumerable = new MultiPropertyClass[]
                {
                    new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 },
                    new MultiPropertyClass() { Property1 = value4, Property2 = value5.ToUpper(), Property3 = value6 }
                }
            };

            var result = sut.Equals(multiPropertyEnumerableClass1, multiPropertyEnumerableClass2);

            result.ShouldBeFalse();
        }

        public class InAnyOrderTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingEnumerableValuesShouldBeTrue(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
            {
                var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a) };
                var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a).Reverse() };

                sut.With(a => a.Enumerable, true);

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingWithDuplicateValuesShouldBeFalse(string value1, string value2, AutoEqualityComparer<EnumerableClass> sut)
            {
                var enumerableClass1 = new EnumerableClass() { Enumerable = new string[] { value1, value2, value2 } };
                var enumerableClass2 = new EnumerableClass() { Enumerable = new string[] { value1, value1, value2 } };

                sut.With(a => a.Enumerable, true);

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeFalse();
            }
        }
    }
}