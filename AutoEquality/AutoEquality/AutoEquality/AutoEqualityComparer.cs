namespace AutoEquality
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Comparison class for comparing if two objects match.
    /// </summary>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <seealso cref="System.Collections.IEqualityComparer" />
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{T}" />
    // Need to implement the non generic version, as this is casted to when handling deep comparisons.
    public class AutoEqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        private static readonly DefaultEqualityComparer DefaultComparer = new DefaultEqualityComparer();
        private List<PropertyInfo> properties = new List<PropertyInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoEqualityComparer{T}"/> class.
        /// </summary>
        /// <remarks>
        /// By default, this will add all properties to the equality comparison.
        /// </remarks>
        public AutoEqualityComparer()
        {
            // Including all the properties by default makes it a lot easier when handling types during a deep comparison.
            this.WithAll();
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// <c>true</c> if the specified objects are equal; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(T x, T y)
        {
            // Quick initial check.  If the objects are the same instance, then there is no need to do any other checking.
            var result = ReferenceEquals(x, y);

            if (!result)
            {
                // Second escape check.  If one of the items is null and the other is not, there is no match.  But items being null
                // is already handled by the ReferenceEquals check above.
                result = (x != null) && (y != null);

                if (result)
                {
                    // Both items are defined, so do configured comparison check.
                    result = this.CompareProperties(x, y);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="x">The <see cref="object" /> to compare with this instance.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public new bool Equals(object x, object y)
        {
            return this.Equals((T)x, (T)y);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
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

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public int GetHashCode(object obj)
        {
            return this.GetHashCode((T)obj);
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        public void With<TProperty>(Expression<Func<T, TProperty>> withProperty)
        {
            if (withProperty == null)
            {
                throw new ArgumentNullException(nameof(withProperty));
            }

            var memberInfo = FindPropertyInfo(withProperty);

            if (!this.properties.Any(a => a.Name == memberInfo.Name))
            {
                this.properties.Add(memberInfo);
            }
        }

        public void With<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> includedProperty, bool inAnyOrder)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Includes all properties in the comparison.
        /// </summary>
        public void WithAll()
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
        }

        /// <summary>
        /// Ignores the specified property from the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withoutProperty">The property to exclude.</param>
        public void Without<TProperty>(Expression<Func<T, TProperty>> withoutProperty)
        {
            if (withoutProperty == null)
            {
                throw new ArgumentNullException(nameof(withoutProperty));
            }

            foreach (var property in this.properties)
            {
                if (property.Name == FindPropertyInfo(withoutProperty).Name)
                {
                    this.properties.Remove(property);

                    break;
                }
            }
        }

        /// <summary>
        /// Excludes all properties from the comparison.
        /// </summary>
        public void WithoutAll()
        {
            this.properties.Clear();
        }

        private static PropertyInfo FindPropertyInfo(Expression expression)
        {
            // TODO: Need to handle this better.
            return ((expression as LambdaExpression).Body as MemberExpression).Member as PropertyInfo;
        }

        private static IEqualityComparer GetComparerForType(Type propertyType)
        {
            IEqualityComparer comparer;

            // If the type implements IEquatable<T>, use this for a comparison.  This handles the language type, e.g. string, int, etc.
            if (ImplementsIEquatable(propertyType))
            {
                comparer = DefaultComparer;
            }
            else if (ImplementsIEnumerable(propertyType))
            {
                comparer = new EnumerableComparer(GetComparerForType(propertyType.GenericTypeArguments.First()));
            }
            else
            {
                // Need to dynamically create a new AutoEqualityComparer from the type of the property currently being processed.
                var comparerType = typeof(AutoEqualityComparer<>).MakeGenericType(propertyType);
                comparer = Activator.CreateInstance(comparerType) as IEqualityComparer;
            }

            return comparer;
        }

        private static bool ImplementsIEnumerable(Type type)
        {
            return type
                .GetInterfaces()
                .Contains(typeof(IEnumerable));
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
                var comparer = GetComparerForType(property.PropertyType);

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