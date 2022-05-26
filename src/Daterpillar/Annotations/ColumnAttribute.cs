using System;

namespace Acklann.Daterpillar.Annotations
{
    /// <summary>
    /// Indicates that a public field or property represents a SQL table column. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Attribute" />
    [AttributeUsage((AttributeTargets.Property | AttributeTargets.Field), AllowMultiple = true, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        public ColumnAttribute() : this(null, null, 0, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColumnAttribute(string name) : this(name, null, 0, 0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="schemaType">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public ColumnAttribute(SchemaType schemaType, int scale = 0, int precision = 0) : this(null, Scripting.Translators.TranslatorBase.ConvertToString(schemaType), scale, precision)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The data-type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public ColumnAttribute(string name, SchemaType type, int scale = 0, int precision = 0) : this(name, Scripting.Translators.TranslatorBase.ConvertToString(type), scale, precision)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="typeName">The data-type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public ColumnAttribute(string name, string typeName, int scale = 0, int precision = 0)
        {
            Name = name;
            Scale = scale;
            TypeName = typeName;
            DefaultValue = null;
            Precision = precision;
        }

        /// <summary>
        /// The name of the column.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>
        /// The scale.
        /// </value>
        public readonly int Scale;

        /// <summary>
        /// Gets or sets the precision.
        /// </summary>
        /// <value>
        /// The precision.
        /// </value>
        public readonly int Precision;

        /// <summary>
        /// Indicates whether the column can be set to <c>null</c>.
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// The default value.
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName { get; set; }

        /// <summary>
        /// Indicates whether the column is auto incremented.
        /// </summary>
        public bool AutoIncrement { get; set; }
    }
}