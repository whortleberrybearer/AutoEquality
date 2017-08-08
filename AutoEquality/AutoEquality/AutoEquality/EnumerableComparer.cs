namespace AutoEquality
{
    using System.Collections;
    using System.Linq;

    // Due to the reflection in the auto comparer, the IEqualityComparer and any IEnumerable instances are all of a non-generic type.  However,
    // due to the use in the AutoEqualityClass, all items will be the same type and will match the type of the typeComparer.
    internal class EnumerableComparer : IEqualityComparer
    {
        private IEqualityComparer typeComparer;
        private bool inAnyOrder;

        internal EnumerableComparer(IEqualityComparer typeComparer, bool inAnyOrder)
        {
            this.typeComparer = typeComparer;
            this.inAnyOrder = inAnyOrder;
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
