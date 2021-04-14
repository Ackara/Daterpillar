using System;

namespace Acklann.Daterpillar.Modeling.Attributes
{
    /// <summary>
    /// Indicates that a enum field represents a table record/row. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class EnumValueAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnumValueAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public EnumValueAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The value name.
        /// </summary>
        public readonly string Name;
    }
}