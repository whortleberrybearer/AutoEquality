namespace AutoEquality
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Comparison class for comparing if two objects match.
    /// </summary>
    /// <remarks>
    /// Inheritors of this class are recommended to derive from <seealso cref="AutoEquality.AutoEqualityComparerBase{T}"/> instead.
    /// This implements the functionality of an auto equality comparer, but does not publicly expose the with / without setup methods.
    /// </remarks>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <seealso cref="AutoEquality.AutoEqualityComparerBase{T}" />
    public sealed class AutoEqualityComparer<T> : AutoEqualityComparerBase<T>
    {
        private static readonly DefaultEqualityComparer DefaultComparer = new DefaultEqualityComparer();
        private Dictionary<string, PropertyConfiguration> properties = new Dictionary<string, PropertyConfiguration>();
        private Dictionary<Type, IEqualityComparer> typeComparers = new Dictionary<Type, IEqualityComparer>();

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
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="withProperty"/> is null.</exception>
        public new void With<TProperty>(Expression<Func<T, TProperty>> withProperty)
        {
            base.With(withProperty);
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <param name="inAnyOrder">If the sequence of <paramref name="withProperty" /> can be in any order.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="withProperty"/> is null.</exception>
        public new void With<TProperty>(Expression<Func<T, IEnumerable<TProperty>>> withProperty, bool inAnyOrder)
        {
            base.With(withProperty, inAnyOrder);
        }

        /// <summary>
        /// Includes all properties in the comparison.
        /// </summary>
        public new void WithAll()
        {
            base.WithAll();
        }

        /// <summary>
        /// Sets the comparer for the specific type.
        /// </summary>
        /// <typeparam name="TComparer">The type of the comparer.</typeparam>
        /// <param name="typeComparer">The type comparer.</param>
        /// <exception cref="System.ArgumentNullException">If <paramref name="typeComparer"/> is null.</exception>
        /// <exception cref="System.InvalidOperationException">IF a comparer for the type has already been set.</exception>
        public void WithComparer<TComparer>(IEqualityComparer<TComparer> typeComparer)
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
        public new void Without<TProperty>(Expression<Func<T, TProperty>> withoutProperty)
        {
            base.Without(withoutProperty);
        }

        /// <summary>
        /// Excludes all properties from the comparison.
        /// </summary>
        public new void WithoutAll()
        {
            base.WithoutAll();
        }

        private IEqualityComparer GetComparerForType(Type propertyType, PropertyConfiguration propertyConfiguration)
        {
            if (!this.typeComparers.TryGetValue(propertyType, out IEqualityComparer comparer))
            {
                // If the type implements IEquatable<T>, use this for a comparison.  This handles the language type, e.g. string, int, etc.
                // Also, if comparing a type of object, the AutoComparer will return true for all items due to not having any properties to match, so using
                // the default comparer makes more sense.
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
                        // within it.  Just use object as the type comparer.
                        if (elementType == null)
                        {
                            elementType = typeof(object);
                        }
                    }

                    // The configuration may be defined for the enumerable.  If it is, extract the values.
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