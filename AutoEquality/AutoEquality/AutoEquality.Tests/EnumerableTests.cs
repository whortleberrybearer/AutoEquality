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
        public void NonMatchingEnumerableValuesShouldBeFalse(IEnumerable<string> values, AutoEqualityComparer<EnumerableClass> sut)
        {
            var enumerableClass1 = new EnumerableClass() { Enumerable = values.Select(a => a.ToLower()) };
            var enumerableClass2 = new EnumerableClass() { Enumerable = values.Select(a => a.ToUpper()) };

            var result = sut.Equals(enumerableClass1, enumerableClass2);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineAutoData]
        public void MatchingDeepEnumerableValuesShouldBeTrue(AutoEqualityComparer<DeepEnumerableClass> sut)
        {
            var result = sut.Equals(deepEnumerableClass1, deepEnumerableClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingDeepEnumerableValuesShouldBeFalse(AutoEqualityComparer<DeepEnumerableClass> sut)
        {
            var result = sut.Equals(deepEnumerableClass1, deepEnumerableClass2);

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