namespace AutoEquality
{
    using System.Collections.Generic;
    using System.Reflection;

    public class AutoEqualityComparer<T> : IEqualityComparer<T>
    {
        private PropertyInfo[] properties;

        public AutoEqualityComparer()
        {
            this.properties = typeof(T).GetProperties();
        }

        public bool Equals(T x, T y)
        {
            var result = true;

            foreach (var property in this.properties)
            {
                if (property.GetValue(x) != property.GetValue(y))
                {
                    result = false;

                    break;
                }
            }

            return result;
        }

        public int GetHashCode(T obj)
        {
            var result = 0;

            foreach (var property in this.properties)
            {
                // I don't think this is a good way of doing this, but its a start.
                result ^= property.GetValue(obj).GetHashCode();
            }

            return result;
        }
    }
}