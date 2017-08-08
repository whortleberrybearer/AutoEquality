namespace AutoEquality
{
    using System.Collections;

    internal class EnumerableComparer : IEqualityComparer
    {
        private IEqualityComparer typeComparer;

        internal EnumerableComparer(IEqualityComparer typeComparer)
        {
            this.typeComparer = typeComparer;
        }

        public new bool Equals(object x, object y)
        {
            var result = true;
            var xEnumerator = ((IEnumerable)x).GetEnumerator();
            var yEnumerator = ((IEnumerable)y).GetEnumerator();
            bool xHasValue;
            bool yHasValue;

            do
            {
                xHasValue = xEnumerator.MoveNext();
                yHasValue = yEnumerator.MoveNext();

                // If either item does not have a value, then the enumerable is a different size and therefore not matching.
                result = xHasValue == yHasValue;

                if (result && xHasValue)
                {
                    result = this.typeComparer.Equals(xEnumerator.Current, yEnumerator.Current);
                }
            }
            while (result && xHasValue);

            return result;
        }

        public int GetHashCode(object obj)
        {
            var result = 0;

            foreach (var item in (IEnumerable)obj)
            {
                result ^= this.typeComparer.GetHashCode(item);
            }

            return result;
        }
    }
}
