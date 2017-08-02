namespace AutoEquality.Tests
{
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class NestedClassTests
    {
        [Theory]
        [InlineAutoData]
        public void MatchingNestedClassShouldBeTrue(string value1, string value2, string value3, string value4, AutoEqualityComparer<NestedClass1> sut)
        {
            var nestedClass1 = new NestedClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Nested = new NestedClass2()
                {
                    Property3 = value3,
                    Property4 = value4
                }
            };
            var nestedClass2 = new NestedClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Nested = new NestedClass2()
                {
                    Property3 = value3,
                    Property4 = value4
                }
            };

            sut.IncludeAll();

            var result = sut.Equals(nestedClass1, nestedClass2);

            result.ShouldBeTrue();
        }

        [Theory]
        [InlineAutoData]
        public void NonMatchingNestedClassShouldBeFalse(string value1, string value2, string value3, string value4, AutoEqualityComparer<NestedClass1> sut)
        {
            var nestedClass1 = new NestedClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Nested = new NestedClass2()
                {
                    Property3 = value3.ToUpper(),
                    Property4 = value4.ToUpper()
                }
            };
            var nestedClass2 = new NestedClass1()
            {
                Property1 = value1,
                Property2 = value2,
                Nested = new NestedClass2()
                {
                    Property3 = value3.ToLower(),
                    Property4 = value4.ToLower()
                }
            };

            sut.IncludeAll();

            var result = sut.Equals(nestedClass1, nestedClass2);

            result.ShouldBeFalse();
        }

        public class NestedClass1
        {
            public NestedClass2 Nested { get; set; }

            public string Property1 { get; set; }

            public string Property2 { get; set; }
        }

        public class NestedClass2
        {
            public string Property3 { get; set; }

            public string Property4 { get; set; }
        }
    }
}