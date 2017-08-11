namespace AutoEquality
{
    using System.Collections;
    using System.Collections.Generic;

    // This wrapper class it needed to allow comparer that implement the generic IEqualityComparer to be called via the reflected type.
    internal class EqualityComparerWrapper<T> : IEqualityComparer
    {
        private IEqualityComparer<T> typeComparer;

        internal EqualityComparerWrapper(IEqualityComparer<T> typeComparer)
        {
            this.typeComparer = typeComparer;
        }

        public new bool Equals(object x, object y)
        {
            return this.typeComparer.Equals((T)x, (T)y);
        }

        public int GetHashCode(object obj)
        {
            return this.typeComparer.GetHashCode((T)obj);
        }
    }
}