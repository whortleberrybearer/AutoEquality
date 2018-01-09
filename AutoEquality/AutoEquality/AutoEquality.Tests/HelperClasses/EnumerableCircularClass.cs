namespace AutoEquality.Tests.HelperClasses
{
    using System.Collections.Generic;

    public class EnumerableCircularClass
    {
        public IEnumerable<EnumerableCircularClass> Circular
        {
            get;
            set;
        }
    }
}