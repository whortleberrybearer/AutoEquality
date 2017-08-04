namespace AutoEquality.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class EnumerableTests
    {
        // TODO: Support for any order./

        public class ContextTodo
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
        }

        public class EnumerableClass
        {
            public IEnumerable<string> Enumerable { get; set; }
        }
    }
}
