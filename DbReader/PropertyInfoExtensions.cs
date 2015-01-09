﻿namespace DbReader
{
    using System.Reflection;

    /// <summary>
    /// Extends the <see cref="PropertyInfo"/> class.
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        /// Determines if the <paramref name="property"/> is writeable.
        /// </summary>
        /// <param name="property">The target <see cref="PropertyInfo"/>.</param>
        /// <returns>true, if the <paramref name="property"/> is writeable, otherwise, false.</returns>
        public static bool IsWriteable(this PropertyInfo property)
        {
            MethodInfo setMethod = property.GetSetMethod();
            return setMethod != null && !setMethod.IsStatic && setMethod.IsPublic;
        }
    }
}