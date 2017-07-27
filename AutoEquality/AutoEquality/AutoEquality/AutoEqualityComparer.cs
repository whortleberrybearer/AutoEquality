namespace AutoEquality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class AutoEqualityComparer<T> : IEqualityComparer<T>
    {
        private List<PropertyInfo> properties;

        public AutoEqualityComparer()
        {
            this.properties = typeof(T).GetProperties().ToList();
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

        public AutoEqualityComparer<T> Ignore<TProperty>(Expression<Func<T, TProperty>> ignoredProperty)
        {
            foreach (var property in this.properties)
            {
                if (property.Name == (ignoredProperty.Body as MemberExpression).Member.Name)
                {
                    this.properties.Remove(property);

                    break;
                }
            }

            return this;
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