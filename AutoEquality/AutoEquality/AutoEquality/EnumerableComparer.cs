﻿namespace AutoEquality
{
    using System;
    using System.Collections;
    using System.Linq;

    // Due to the reflection in the auto comparer, the IEqualityComparer and any IEnumerable instances are all of a non-generic type.  However,
    // due to the use in the AutoEqualityClass, all items will be the same type and will match the type of the typeComparer.
    internal class EnumerableComparer : IEqualityComparer
    {
        private bool inAnyOrder;
        private IEqualityComparer typeComparer;

        internal EnumerableComparer(IEqualityComparer typeComparer, bool inAnyOrder)
        {
            this.typeComparer = typeComparer;
            this.inAnyOrder = inAnyOrder;
        }

        public new bool Equals(object x, object y)
        {
            // Converting to an array list makes it easier to track if there are multiple elements that are duplicated as they can
            // be removed from the list.
            var xArrayList = MakeArrayList((IEnumerable)x);
            var yArrayList = MakeArrayList((IEnumerable)y);

            // Quick escape check.  If they are not the same length, the are not equal.
            var result = xArrayList.Count == yArrayList.Count;

            if (result)
            {
                for (var i = 0; i < xArrayList.Count; i++)
                {
                    if (!this.typeComparer.Equals(xArrayList[i], yArrayList[i]))
                    {
                        result = false;

                        break;
                    }
                }
            }

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

        private static ArrayList MakeArrayList(IEnumerable enumerable)
        {
            var arrayList = new ArrayList();

            foreach (var obj in enumerable)
            {
                arrayList.Add(obj);
            }

            return arrayList;
        }
    }
}