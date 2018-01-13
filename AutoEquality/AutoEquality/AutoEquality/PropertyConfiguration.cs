namespace AutoEquality
{
    using System.Collections;
    using System.Reflection;

    internal class PropertyConfiguration
    {
        internal IEqualityComparer Comparer { get; set; }

        internal PropertyInfo PropertyInfo { get; set; }
    }
}