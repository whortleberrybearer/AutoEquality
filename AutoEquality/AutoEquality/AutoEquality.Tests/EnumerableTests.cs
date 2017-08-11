namespace AutoEquality.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    // Due to the reflection internally in the comparer, the tests are broken out into various blocks to cover different instances.
    // The majority of the tests are within the generic type tests, but there are also some additional basic checks to validate that
    // a non generic IEnumerable and arrays also work.
    public class EnumerableTests
    {
        public class ArrayTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingArrayValuesShouldBeTrue(IEnumerable<string> values, AutoEqualityComparer<ArrayClass> sut)
            {
                var arrayClass1 = new ArrayClass() { Array = values.Select(a => a).ToArray() };
                var arrayClass2 = new ArrayClass() { Array = values.Select(a => a).ToArray() };

                var result = sut.Equals(arrayClass1, arrayClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingArrayValuesShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<ArrayClass> sut)
            {
                var arrayClass1 = new ArrayClass() { Array = values.Select(a => a.ToLower()).ToArray() };
                var arrayClass2 = new ArrayClass() { Array = values.Select(a => a.ToUpper()).ToArray() };

                var result = sut.Equals(arrayClass1, arrayClass2);

                result.ShouldBeFalse();
            }
        }

        public class GenericEnumerableTests
        {
            [Theory]
            [InlineAutoData]
            public void DifferentLengthsShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<GenericEnumerableClass> sut)
            {
                // Skipping and reversing the list means the start elements match, the missing element is the one at the end.
                var enumerableClass1 = new GenericEnumerableClass() { Enumerable = values.Select(a => a).Reverse() };
                var enumerableClass2 = new GenericEnumerableClass() { Enumerable = values.Select(a => a).Skip(1).Reverse() };

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeFalse();
            }

            [Theory]
            [InlineAutoData]
            public void MatchingEnumerableValuesShouldBeTrue(IEnumerable<string> values, AutoEqualityComparer<GenericEnumerableClass> sut)
            {
                var enumerableClass1 = new GenericEnumerableClass() { Enumerable = values.Select(a => a) };
                var enumerableClass2 = new GenericEnumerableClass() { Enumerable = values.Select(a => a) };

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
            public void NonMatchingEnumerableValuesShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<GenericEnumerableClass> sut)
            {
                var enumerableClass1 = new GenericEnumerableClass() { Enumerable = values.Select(a => a.ToLower()) };
                var enumerableClass2 = new GenericEnumerableClass() { Enumerable = values.Select(a => a.ToUpper()) };

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
        }

        public class InAnyOrderTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingEnumerableValuesShouldBeTrue(IEnumerable<string> values, AutoEqualityComparer<GenericEnumerableClass> sut)
            {
                var enumerableClass1 = new GenericEnumerableClass() { Enumerable = values.Select(a => a) };
                var enumerableClass2 = new GenericEnumerableClass() { Enumerable = values.Select(a => a).Reverse() };

                sut.With(a => a.Enumerable, true);

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingWithDuplicateValuesShouldBeFalse(string value1, string value2, AutoEqualityComparer<GenericEnumerableClass> sut)
            {
                var enumerableClass1 = new GenericEnumerableClass() { Enumerable = new string[] { value1, value2, value2 } };
                var enumerableClass2 = new GenericEnumerableClass() { Enumerable = new string[] { value1, value1, value2 } };

                sut.With(a => a.Enumerable, true);

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeFalse();
            }
        }

        public class NonGenericEnumerableTests
        {
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
            public void NonMatchingEnumerableValuesShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
            {
                var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a.ToLower()) };
                var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a.ToUpper()) };

                var result = sut.Equals(enumerableClass1, enumerableClass2);

                result.ShouldBeFalse();
            }
        }
    }
}