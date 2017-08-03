namespace AutoEquality
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    // Need to implement the non generic version, as this is casted to when handling deep comparisons.
    public class AutoEqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        private static readonly DefaultEqualityComparer DefaultComparer = new DefaultEqualityComparer();
        private List<PropertyInfo> properties = new List<PropertyInfo>();

        public AutoEqualityComparer()
        {
            // Including all the properties by default makes it a lot easier when handling types during a deep comparison.
            this.IncludeAll();
        }

        public bool Equals(T x, T y)
        {
            // Quick initial check.  If the objects are the same instance, then there is no need to do any other checking.
            var result = ReferenceEquals(x, y);

            if (!result)
            {
                result = this.CompareProperties(x, y);
            }

            return result;
        }

        public new bool Equals(object x, object y)
        {
            return this.Equals((T)x, (T)y);
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

        public int GetHashCode(object obj)
        {
            return this.GetHashCode((T)obj);
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

        private static bool ImplementsIEquatable(Type type)
        {
            return type
                .GetInterfaces()
                .Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEquatable<>));
        }

        private bool CompareProperties(T x, T y)
        {
            var result = true;

            foreach (var property in this.properties)
            {
                IEqualityComparer comparer;

                // If the type implements IEquatable<T>, use this for a comparison.  This handles the language type, e.g. string, int, etc.
                if (ImplementsIEquatable(property.PropertyType))
                {
                    comparer = DefaultComparer;
                }
                else
                {
                    // Need to dynamically create a new AutoEqualityComparer from the type of the property currently being processed.
                    var comparerType = typeof(AutoEqualityComparer<>).MakeGenericType(property.PropertyType);
                    comparer = Activator.CreateInstance(comparerType) as IEqualityComparer;
                }

                if (!comparer.Equals(property.GetValue(x), property.GetValue(y)))
                {
                    result = false;

                    break;
                }
            }

            return result;
        }
    }
}