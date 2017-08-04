namespace AutoEquality.Tests
{
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class DeepClassTests
    {
        [Theory]
        [InlineAutoData]
        public void MatchingDeepClassShouldBeTrue(string value1, string value2, string value3, string value4, AutoEqualityComparer<DeepClass1> sut)
        {
            var deepClass1 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3,
                    Property4 = value4
                }
            };
            var deepClass2 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3,
                    Property4 = value4
                }
            };

            var result = sut.Equals(deepClass1, deepClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingDeepClassShouldBeFalse(string value1, string value2, string value3, string value4, AutoEqualityComparer<DeepClass1> sut)
        {
            var deepClass1 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3.ToUpper(),
                    Property4 = value4.ToUpper()
                }
            };
            var deepClass2 = new DeepClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Deep = new DeepClass2()
                {
                    Property3 = value3.ToLower(),
                    Property4 = value4.ToLower()
                }
            };

            var result = sut.Equals(deepClass1, deepClass2);

            result.ShouldBeFalse();
        }
    }
}