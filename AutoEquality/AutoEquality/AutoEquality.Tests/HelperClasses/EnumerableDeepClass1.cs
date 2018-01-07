namespace AutoEquality.Tests.HelperClasses
{
    using System.Collections.Generic;

    public class EnumerableDeepClass1
    {
        public IEnumerable<DeepClass2> Deep { get; set; }

        public string Property1 { get; set; }

        public string Property2 { get; set; }
    }
}