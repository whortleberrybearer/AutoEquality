namespace AutoEquality.Tests
{
    using System;
    using System.Collections.Generic;
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class TypeComparerTests
    {
        // TODO: Without comparer tests.
        [Theory]
        [InlineAutoData]
        public void MatchingTypeShouldBeTrue(string value1, string value2, string value3, string value4, AutoEqualityComparer<DeepClass2> typeComparer, AutoEqualityComparer<DeepClass1> sut)
        {
            var deepClass1 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3,
                    Property4 = value4.ToUpper()
                }
            };
            var deepClass2 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3,
                    Property4 = value4.ToLower()
                }
            };

            typeComparer.Without(a => a.Property4);

            sut.WithComparer(typeComparer);

            var result = sut.Equals(deepClass1, deepClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NullComparerShouldThrowArguementNullException(AutoEqualityComparer<object> sut)
        {
            Should.Throw<ArgumentNullException>(() => sut.WithComparer((IEqualityComparer<string>)null));
        }
    }
}