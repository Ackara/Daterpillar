using System;

namespace Acklann.Daterpillar.Attributes
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
        /// <param name="query">The query.</param>
        public ViewAttribute(string query) : this(null, query)
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
            Query = query ?? throw new ArgumentNullException(nameof(query));
            Arguments = args;
            Name = name;
        }

        /// <summary>
        /// The name
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The SQL query that will generate the view.
        /// </summary>
        public readonly string Query;

        /// <summary>
        /// The query arguments.
        /// </summary>
        public readonly object[] Arguments;
    }
}