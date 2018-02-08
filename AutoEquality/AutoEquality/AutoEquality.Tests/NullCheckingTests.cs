namespace AutoEquality.Tests
{
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class NullCheckingTests
    {
        [Theory]
        [InlineAutoData]
        public void BothNullShouldBeTrue(AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var result = sut.Equals(null, null);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingNullPropertyShouldBeFalse(string value1, string value2, string value3, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var simpleClass1 = new MultiPropertyClass() { Property1 = value1, Property2 = value2, Property3 = value3 };
            var simpleClass2 = new MultiPropertyClass() { Property1 = value1, Property2 = null, Property3 = value3 };

            var result = sut.Equals(simpleClass1, simpleClass2);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineAutoData]
        public void NullArrayPropertiesShouldBeTrue(AutoEqualityComparer<ArrayClass> sut)
        {
            var arrayClass1 = new ArrayClass();
            var arrayClass2 = new ArrayClass();

            var result = sut.Equals(arrayClass1, arrayClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NullItemShouldBeFalse(MultiPropertyClass multiPropertyClass, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var result = sut.Equals(multiPropertyClass, null);

            result.ShouldBeFalse();
        }

        [Theory]
        [InlineAutoData]
        public void NullPropertiesShouldBeTrue(AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var multiPropertyClass1 = new MultiPropertyClass();
            var multiPropertyClass2 = new MultiPropertyClass();

            var result = sut.Equals(multiPropertyClass1, multiPropertyClass2);

            result.ShouldBeTrue();
        }
    }
}