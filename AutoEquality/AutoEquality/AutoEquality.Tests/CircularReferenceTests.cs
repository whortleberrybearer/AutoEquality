namespace AutoEquality.Tests
{
    using System;
    using System.Linq.Expressions;
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class CircularReferenceTests
    {
        [Theory]
        [InlineAutoData]
        public void SingleLevelNestingShouldBeTrue(AutoEqualityComparer<CircularClass> sut)
        {
            var circularClass1 = new CircularClass();
            var circularClass2 = new CircularClass();

            circularClass1.Circular = circularClass1;
            circularClass2.Circular = circularClass2;

            var result = sut.Equals(circularClass1, circularClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void MultiLevelNestingShouldBeTrue(AutoEqualityComparer<CircularClass> sut)
        {
            var circularClass1 = new CircularClass();
            var circularClass2 = new CircularClass();
            var circularClass3 = new CircularClass();
            var circularClass4 = new CircularClass();

            circularClass1.Circular = circularClass2;
            circularClass2.Circular = circularClass1;
            circularClass3.Circular = circularClass4;
            circularClass4.Circular = circularClass3;

            var result = sut.Equals(circularClass1, circularClass3);

            result.ShouldBeTrue();
        }
    }
}
