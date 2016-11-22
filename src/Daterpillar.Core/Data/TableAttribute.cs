using System;

namespace Acklann.Daterpillar.Data
{
    /// <summary>
    /// Specifies the database table that a class is mapped to.
    /// </summary>
    /// <seealso cref="System.Attribute"/>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public TableAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// The name of the table the class is mapped to.
        /// </summary>
        /// <value>The table name.</value>
        public readonly string Name;
    }
}