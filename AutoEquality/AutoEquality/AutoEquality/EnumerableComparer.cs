namespace AutoEquality
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
            var result = (x != null) && (y != null);

            if (result)
            {
                // Converting to an array list makes it easier to track if there are multiple elements that are duplicated as they can
                // be removed from the list.
                var xArrayList = MakeArrayList((IEnumerable)x);
                var yArrayList = MakeArrayList((IEnumerable)y);

                // Quick escape check.  If they are not the same length, the are not equal.
                result = xArrayList.Count == yArrayList.Count;

                if (result)
                {
                    for (var i = 0; i < xArrayList.Count; i++)
                    {
                        if (!this.inAnyOrder)
                        {
                            result = this.typeComparer.Equals(xArrayList[i], yArrayList[i]);
                        }
                        else
                        {
                            // Cant use ArrayList.Contains as need to use the comparer to match equality.
                            var yIndex = this.FindItemInArray(xArrayList[i], yArrayList);

                            if (yIndex > -1)
                            {
                                // Remove the item from the list to prevent false matches due to duplicate items in the list.
                                yArrayList.RemoveAt(yIndex);
                            }
                            else
                            {
                                result = false;
                            }
                        }

                        if (!result)
                        {
                            break;
                        }
                    }
                }
            }
            else if ((x == null) && (y == null))
            {
                // If both items are null, then this is a match.
                result = true;
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

        private int FindItemInArray(object obj, ArrayList arrayList)
        {
            var result = -1;

            for (var i = 0; i < arrayList.Count; i++)
            {
                if (this.typeComparer.Equals(obj, arrayList[i]))
                {
                    result = i;

                    break;
                }
            }

            return result;
        }
    }
}