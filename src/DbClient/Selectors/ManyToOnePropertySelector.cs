namespace DbClient.Selectors
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Extensions;

    /// <summary>
    /// A <see cref="IPropertySelector"/> that selects properties 
    /// that represents a "Many to One" navigation property.
    /// </summary>
    public class ManyToOnePropertySelector : IPropertySelector
    {
        /// <summary>
        /// Executes the selector and returns a list of properties.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>An array of <see cref="PropertyInfo"/> instances.</returns>
        public PropertyInfo[] Execute(Type type)
        {
            return
                type.GetProperties()
                    .Where(p => !TypeReflectionExtensions.IsSimpleType(p.PropertyType) && !TypeReflectionExtensions.IsEnumerable(p.PropertyType) && PropertyReflectionExtensions.IsWriteable(p))
                    .ToArray();
        }
    }
}