namespace AutoEquality
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class AutoEqualityComparer<T> : IEqualityComparer<T>
    {
        private List<PropertyInfo> properties = new List<PropertyInfo>();

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

        public AutoEqualityComparer<T> Ignore<TProperty>(Expression<Func<T, TProperty>> ignoredProperty)
        {
            foreach (var property in this.properties)
            {
                if (property.Name == FindPropertyInfo(ignoredProperty).Name)
                {
                    this.properties.Remove(property);

                    break;
                }
            }

            return this;
        }

        public AutoEqualityComparer<T> IgnoreAll()
        {
            this.properties.Clear();

            return this;
        }

        public AutoEqualityComparer<T> Include<TProperty>(Expression<Func<T, TProperty>> includedProperty)
        {
            var memberInfo = FindPropertyInfo(includedProperty);

            if (!this.properties.Any(a => a.Name == memberInfo.Name))
            {
                this.properties.Add(memberInfo);
            }

            return this;
        }

        public AutoEqualityComparer<T> IncludeAll()
        {
            if (!this.properties.Any())
            {
                this.properties.AddRange(typeof(T).GetProperties());
            }
            else
            {
                foreach (var propertyInfo in typeof(T).GetProperties())
                {
                    if (!this.properties.Any(a => a.Name == propertyInfo.Name))
                    {
                        this.properties.Add(propertyInfo);
                    }
                }
            }

            return this;
        }

        private static PropertyInfo FindPropertyInfo(Expression expression)
        {
            // TODO: Need to handle this better.
            return ((expression as LambdaExpression).Body as MemberExpression).Member as PropertyInfo;
        }
    }
}