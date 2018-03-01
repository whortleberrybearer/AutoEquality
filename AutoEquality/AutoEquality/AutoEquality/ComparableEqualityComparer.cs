namespace AutoEquality
{
    using System;
    using System.Collections;

    // Helper class to allow comparison using an IComparable. This is needed for enum support as they don't implement any other of the
    // equality interfaces.
    internal class ComparableEqualityComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            bool result;

            if (x != null)
            {
                result = ((IComparable)x).CompareTo(y) == 0;
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