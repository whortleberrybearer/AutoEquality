namespace AutoEquality
{
    using System.Collections;

    // This is just a little helper class to allow the use of a comparer to perform the standard equals method and make the comparison code
    // slightly cleaner.
    internal class DefaultEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
