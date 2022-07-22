using System;

namespace Acklann.Daterpillar.Annotations
{
    /// <summary>
    /// Indicates that a class or struct represents a SQL View. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage((AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface), AllowMultiple = false, Inherited = true)]
    public sealed class ViewAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the view table.</param>
        public ViewAttribute(string name) : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewAttribute" /> class.
        /// </summary>
        /// <param name="name">The name of the view.</param>
        /// <param name="query">The query that will generate the view.</param>
        /// <param name="args">The query arguments.</param>
        /// <exception cref="ArgumentNullException">query</exception>
        public ViewAttribute(string name, string query, params object[] args)
        {
            Name = name;
            Query = args?.Length > 0 ? string.Format(query, args) : query;
        }

        /// <summary>
        /// The name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The SQL query that will generate the view.
        /// </summary>
        public readonly string Query;
    }
}