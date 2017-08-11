namespace AutoEquality
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Comparison class for comparing if two objects match.
    /// </summary>
    /// <remarks>
    /// Inheritors of this class are recommended to derive from <seealso
    /// cref="AutoEquality.AutoEqualityComparerBase{T}"/> instead. This implements the functionality
    /// of an auto equality comparer, but does not publicly expose the with / without setup methods.
    /// </remarks>
    /// <typeparam name="T">The type of objects to compare.</typeparam>
    /// <seealso cref="AutoEquality.AutoEqualityComparerBase{T}"/>
    public sealed class AutoEqualityComparer<T> : AutoEqualityComparerBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoEqualityComparer{T}"/> class.
        /// </summary>
        /// <remarks>By default, this will add all properties to the equality comparison.</remarks>
        public AutoEqualityComparer()
        {
            // Including all the properties by default makes it a lot easier when handling types
            // during a deep comparison.
            this.WithAll();
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="withProperty"/> is null.
        /// </exception>
        public new void With<TProperty>(Expression<Func<T, TProperty>> withProperty)
        {
            base.With(withProperty);
        }

        /// <summary>
        /// Includes the specified property in the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withProperty">The property to include.</param>
        /// <param name="inAnyOrder">
        /// If the sequence of <paramref name="withProperty"/> can be in any order.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="withProperty"/> is null.
        /// </exception>
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
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="typeComparer"/> is null.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// If a comparer for the type has already been set.
        /// </exception>
        public new void WithComparer<TComparer>(IEqualityComparer<TComparer> typeComparer)
        {
            base.WithComparer(typeComparer);
        }

        /// <summary>
        /// Ignores the specified property from the comparison.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="withoutProperty">The property to exclude.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="withoutProperty"/> is null.
        /// </exception>
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

        /// <summary>
        /// Removes the comparer for the given type.
        /// </summary>
        /// <param name="comparerType">The type of the comparer to remove.</param>
        /// <exception cref="System.ArgumentNullException">
        /// If <paramref name="comparerType"/> is null.
        /// </exception>
        public new void WithoutComparer(Type comparerType)
        {
            base.WithoutComparer(comparerType);
        }
    }
}