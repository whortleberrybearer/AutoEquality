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
    /// <seealso cref="System.Collections.IEqualityComparer"/>
    /// <seealso cref="System.Collections.Generic.IEqualityComparer{T}"/>
    // Need to implement the non generic version, as this is casted to when handling deep comparisons.
    public abstract class AutoEqualityComparerBase<T> : IEqualityComparer, IEqualityComparer<T>
        where T : class
    {
        private static readonly DefaultEqualityComparer DefaultComparer = new DefaultEqualityComparer();
        private Dictionary<string, PropertyConfiguration> properties = new Dictionary<string, PropertyConfiguration>();
        private Dictionary<Type, IEqualityComparer> typeComparers = new Dictionary<Type, IEqualityComparer>();

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type <paramref name="T"/> to compare.</param>
        /// <param name="y">The second object of type <paramref name="T"/> to compare.</param>
        /// <returns><c>true</c> if the specified objects are equal; otherwise, <c>false</c>.</returns>
        public bool Equals(T x, T y)
        {
            // Quick initial check. If the objects are the same instance, then there is no need to do any other checking.
            var result = ReferenceEquals(x, y);

            if (!result)
            {
                // Second escape check. If one of the items is null and the other is not, there is no match. But items being null is already
                // handled by the ReferenceEquals check above.
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
        /// Determines whether the specified <see cref="object"/>, is equal to this instance.
        /// </summary>
        /// <param name="x">The <see cref="object"/> to compare with this instance.</param>
        /// <param name="y">The y.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to this instance; otherwise, <c>false</c>.</returns>
        public new bool Equals(object x, object y)
        {
            // Need to ensure that both are the correct type, otherwise the "as" command below will convert the values to null, and
            // the two nulls evaluate to true.
            var result = (x is T) && (y is T);

            if (result)
            {
                result = this.Equals(x as T, y as T);
            }

            return result;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(T obj)
        {
            var result = 0;

            foreach (var property in this.properties.Values)
            {
                // I don't think this is a good way of doing this, but its a start.
                result ^= property.PropertyInfo.GetValue(obj).GetHashCode();
            }

            return result;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public int GetHashCode(object obj)
        {
            return this.GetHashCode(obj as T);
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <param name="comparer">A comparer to use for this property.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="withProperty"/> is null.</exception>
        protected void With<TProperty>(Expression<Func<T, TProperty>> withProperty, IEqualityComparer<TProperty> comparer = null)
        {
            if (withProperty == null)
            {
                throw new ArgumentNullException(nameof(withProperty));
            }

            // Later on, the comparer generic and non-generic versions become a problem, so wrapping it allows casting between each.
            var comparerWrapper = (comparer != null) ? new EqualityComparerWrapper<TProperty>(comparer) : null;

            this.AddToPropertiesIfRequired(FindPropertyInfo(withProperty), new PropertyConfiguration() { Comparer = comparerWrapper });
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <param name="inAnyOrder">If the sequence of <paramref name="withProperty"/> can be in any order.</param>
        /// <param name="comparer">A comparer to use for this property.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="withProperty"/> is null.</exception>
        protected void With<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> withProperty, bool inAnyOrder = false, IEqualityComparer<TProperty> comparer = null)
        {
            if (withProperty == null)
            {
                throw new ArgumentNullException(nameof(withProperty));
            }

            // Later on, the comparer generic and non-generic versions become a problem, so wrapping it allows casting between each.
            var comparerWrapper = (comparer != null) ? new EqualityComparerWrapper<TProperty>(comparer) : null;
            var propertyInfo = FindPropertyInfo(withProperty);

            this.AddToPropertiesIfRequired(propertyInfo, new EnumerablePropertyConfiguration() { InAnyOrder = inAnyOrder, Comparer = comparerWrapper });
        }

        /// <summary>
        /// Includes all properties in the comparison.
        /// </summary>
        protected void WithAll()
        {
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                this.AddToPropertiesIfRequired(propertyInfo);
            }
        }

        /// <summary>
        /// Sets the comparer for the specific type.
        /// </summary>
        /// <typeparam name="TComparer">The type of the comparer.</typeparam>
        /// <param name="typeComparer">The type comparer.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="typeComparer"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">If a comparer for the type has already been set.</exception>
        protected void WithComparer<TComparer>(IEqualityComparer<TComparer> typeComparer)
        {
            if (typeComparer == null)
            {
                throw new ArgumentNullException(nameof(typeComparer));
            }

            if (!this.typeComparers.ContainsKey(typeof(TComparer)))
            {
                this.typeComparers.Add(typeof(TComparer), new EqualityComparerWrapper<TComparer>(typeComparer));
            }
            else
            {
                throw new InvalidOperationException($"A comparer for type {typeof(TComparer).Name} has already been defined.");
            }
        }

        /// <summary>
        /// Ignores the specified property from the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withoutProperty">The property to exclude.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="withoutProperty"/> is null.</exception>
        protected void Without<TProperty>(Expression<Func<T, TProperty>> withoutProperty)
        {
            if (withoutProperty == null)
            {
                throw new ArgumentNullException(nameof(withoutProperty));
            }

            this.properties.Remove(FindPropertyInfo(withoutProperty).Name);
        }

        /// <summary>
        /// Excludes all properties from the comparison.
        /// </summary>
        protected void WithoutAll()
        {
            this.properties.Clear();
        }

        /// <summary>
        /// Removes the comparer for the given type.
        /// </summary>
        /// <param name="comparerType">The type of the comparer to remove.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="comparerType"/> is null.</exception>
        protected void WithoutComparer(Type comparerType)
        {
            if (comparerType == null)
            {
                throw new ArgumentNullException(nameof(comparerType));
            }

            // Need to find the type of the IEqualityComparer<T> passed into this methods as that is the key to the dictionary. If it does
            // not implement that interface, it will not exist in the list, so we don't need to worry about it.
            if (ImplementsIEqualityComparer(comparerType))
            {
                this.typeComparers.Remove(comparerType.GenericTypeArguments.First());
            }
        }

        private static PropertyInfo FindPropertyInfo(Expression expression)
        {
            // TODO: Need to handle this better.
            return ((expression as LambdaExpression).Body as MemberExpression).Member as PropertyInfo;
        }

        private static bool ImplementsIEnumerable(Type type)
        {
            return type == typeof(IEnumerable) || type.GetInterfaces().Contains(typeof(IEnumerable));
        }

        private static bool ImplementsIEqualityComparer(Type type)
        {
            return type
                .GetInterfaces()
                .Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEqualityComparer<>));
        }

        private static bool ImplementsIEquatable(Type type)
        {
            return type
                .GetInterfaces()
                .Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IEquatable<>));
        }

        private void AddToPropertiesIfRequired(PropertyInfo propertyInfo, PropertyConfiguration propertyConfiguration = null)
        {
            if (!this.properties.ContainsKey(propertyInfo.Name))
            {
                this.properties.Add(propertyInfo.Name, null);
            }

            if (propertyConfiguration == null)
            {
                propertyConfiguration = new PropertyConfiguration();
            }

            propertyConfiguration.PropertyInfo = propertyInfo;

            this.properties[propertyInfo.Name] = propertyConfiguration;
        }

        private bool CompareProperties(T x, T y)
        {
            var result = true;

            foreach (var propertyConfiguration in this.properties.Values)
            {
                var comparer = this.GetComparerForProperty(propertyConfiguration);

                if (!comparer.Equals(propertyConfiguration.PropertyInfo.GetValue(x), propertyConfiguration.PropertyInfo.GetValue(y)))
                {
                    result = false;

                    break;
                }
            }

            return result;
        }

        private IEqualityComparer GetComparerForProperty(PropertyConfiguration propertyConfiguration)
        {
            IEqualityComparer comparer;

            if (propertyConfiguration.Comparer != null)
            {
                // The property type on an enumerable property is the containing type, so can build an enumerable comparer from these details.
                if (ImplementsIEnumerable(propertyConfiguration.PropertyInfo.PropertyType))
                {
                    var enumerablePropertyConfiguration = propertyConfiguration as EnumerablePropertyConfiguration;

                    comparer = new EnumerableComparer(
                        propertyConfiguration.Comparer,
                        enumerablePropertyConfiguration != null ? enumerablePropertyConfiguration.InAnyOrder : false);
                }
                else
                {
                    comparer = propertyConfiguration.Comparer;
                }
            }
            else
            {
                comparer = this.GetComparerForType(propertyConfiguration.PropertyInfo.PropertyType, propertyConfiguration);
            }

            return comparer;
        }

        private IEqualityComparer GetComparerForType(Type propertyType, PropertyConfiguration propertyConfiguration)
        {
            if (!this.typeComparers.TryGetValue(propertyType, out IEqualityComparer comparer))
            {
                // If the type implements IEquatable<T>, use this for a comparison. This handles the language type, e.g. string, int, etc.
                // Also, if comparing a type of object, the AutoComparer will return true for all items due to not having any properties to
                // match, so using the default comparer makes more sense.
                if (ImplementsIEquatable(propertyType) || (propertyType == typeof(object)))
                {
                    comparer = DefaultComparer;
                }
                else if (ImplementsIEnumerable(propertyType))
                {
                    Type elementType;

                    if (propertyType.IsGenericType)
                    {
                        elementType = propertyType.GenericTypeArguments.First();
                    }
                    else
                    {
                        elementType = propertyType.GetElementType();

                        // If the property is just an IEnumerable, it will not be possible to determine the type of the elements contained
                        // within it. Just use object as the type comparer.
                        if (elementType == null)
                        {
                            elementType = typeof(object);
                        }
                    }

                    // The configuration may be defined for the enumerable. If it is, extract the values.
                    var enumerablePropertyConfiguration = propertyConfiguration as EnumerablePropertyConfiguration;

                    comparer = new EnumerableComparer(
                        this.GetComparerForType(elementType, propertyConfiguration),
                        enumerablePropertyConfiguration != null ? enumerablePropertyConfiguration.InAnyOrder : false);
                }
                else
                {
                    // Need to dynamically create a new AutoEqualityComparer from the type of the property currently being processed.
                    var comparerType = typeof(AutoEqualityComparer<>).MakeGenericType(propertyType);
                    comparer = Activator.CreateInstance(comparerType) as IEqualityComparer;
                }
            }

            return comparer;
        }
    }
}