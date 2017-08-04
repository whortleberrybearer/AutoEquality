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
        public void NullItemShouldBeFalse(MultiPropertyClass multiPropertyClass, AutoEqualityComparer<MultiPropertyClass> sut)
        {
            var result = sut.Equals(multiPropertyClass, null);

            result.ShouldBeFalse();
        }
    }
}