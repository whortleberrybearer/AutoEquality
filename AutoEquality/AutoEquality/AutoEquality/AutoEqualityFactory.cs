namespace AutoEquality
{
    using System.Collections.Generic;

    public class AutoEqualityFactory
    {
        public static IEqualityComparer<T> Create<T>()
        {
            return new AutoEqualityComparer<T>();
        }
    }
}
