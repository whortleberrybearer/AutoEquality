namespace AutoEquality
{
    using System.Collections;

    // This is just a little helper class to allow the use of a comparer to perform the standard equals method and make the comparison code
    // slightly cleaner.
    internal class DefaultEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            bool result;

            if (x != null)
            {
                result = x.Equals(y);
            }
            else
            {
                // If y is also null, then this is a match.
                result = y == null;
            }

            return result;
        }

        public int GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }
    }
}
