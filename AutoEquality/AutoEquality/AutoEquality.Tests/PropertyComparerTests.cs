namespace AutoEquality.Tests
{
    using AutoEquality.Tests.HelperClasses;
    using Ploeh.AutoFixture.Xunit2;
    using Shouldly;
    using Xunit;

    public class PropertyComparerTests
    {
        public class WithTests
        {
            [Theory]
            [InlineAutoData]
            public void MatchingEnumerablePropertyShouldBeTrue(
                string value1,
                string value2,
                string value3,
                string value4,
                AutoEqualityComparer<DeepClass2> typeComparer,
                AutoEqualityComparer<EnumerableDeepClass1> sut)
            {
                var deepClass1 = new EnumerableDeepClass1()
                {
                    Property1 = value1,
                    Property2 = value2,
                    Deep = new DeepClass2[]
                    {
                        new DeepClass2()
                        {
                            Property3 = value3,
                            Property4 = value4.ToUpper()
                        }
                    }
                };
                var deepClass2 = new EnumerableDeepClass1()
                {
                    Property1 = value1,
                    Property2 = value2,
                    Deep = new DeepClass2[]
                    {
                        new DeepClass2()
                        {
                            Property3 = value3,
                            Property4 = value4.ToLower()
                        }
                    }
                };

                typeComparer.Without(a => a.Property4);

                sut.With(a => a.Deep, comparer: typeComparer);

                var result = sut.Equals(deepClass1, deepClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void MatchingPropertyShouldBeTrue(
                string value1,
                string value2,
                string value3,
                string value4,
                AutoEqualityComparer<DeepClass2> typeComparer,
                AutoEqualityComparer<DeepClass1> sut)
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

                sut.With(a => a.Deep, typeComparer);

                var result = sut.Equals(deepClass1, deepClass2);

                result.ShouldBeTrue();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingEnumerablePropertyShouldBeFalse(
                string value1,
                string value2,
                string value3,
                string value4,
                AutoEqualityComparer<DeepClass2> typeComparer,
                AutoEqualityComparer<EnumerableDeepClass1> sut)
            {
                var deepClass1 = new EnumerableDeepClass1()
                {
                    Property1 = value1,
                    Property2 = value2,
                    Deep = new DeepClass2[]
                    {
                        new DeepClass2()
                        {
                            Property3 = value3,
                            Property4 = value4.ToUpper()
                        }
                    }
                };
                var deepClass2 = new EnumerableDeepClass1()
                {
                    Property1 = value1,
                    Property2 = value2,
                    Deep = new DeepClass2[]
                    {
                        new DeepClass2()
                        {
                            Property3 = value3,
                            Property4 = value4.ToLower()
                        }
                    }
                };

                sut.With(a => a.Deep, comparer: typeComparer);

                var result = sut.Equals(deepClass1, deepClass2);

                result.ShouldBeFalse();
            }

            [Theory]
            [InlineAutoData]
            public void NonMatchingPropertyShouldBeFalse(
                string value1,
                string value2,
                string value3,
                string value4,
                AutoEqualityComparer<DeepClass2> typeComparer,
                AutoEqualityComparer<DeepClass1> sut)
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

                sut.With(a => a.Deep, typeComparer);

                var result = sut.Equals(deepClass1, deepClass2);

                result.ShouldBeFalse();
            }
        }
    }
}