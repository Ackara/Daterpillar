using System;

namespace Acklann.Daterpillar.Annotations
{
    /// <summary>
    /// Indicates that a class or enum represents a SQL table but not include in the database. This class cannot be inherited.
    /// </summary>
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct | AttributeTargets.Enum), AllowMultiple = false, Inherited = false)]
    public sealed class TempTableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TempTableAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the table</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TempTableAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// The name of the table.
        /// </summary>
        public readonly string Name;
    }
}