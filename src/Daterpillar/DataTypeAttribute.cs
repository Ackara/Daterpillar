﻿using System;

namespace Acklann.Daterpillar
{
    /// <summary>
    /// Represents a column's data-type. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class DataTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeAttribute"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public DataTypeAttribute(SchemaType type, int scale = 0, int precision = 0) : this(Configuration.DataType.ToString(type), scale, precision)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTypeAttribute"/> class.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="precision">The precision.</param>
        public DataTypeAttribute(string typeName, int scale = 0, int precision = 0)
        {
            Precision = precision;
            TypeName = typeName;
            Scale = scale;
        }

        /// <summary>
        /// The type name
        /// </summary>
        public readonly string TypeName;

        /// <summary>
        /// The scale
        /// </summary>
        public readonly int Scale;

        /// <summary>
        /// The precision
        /// </summary>
        public readonly int Precision;

        /// <summary>
        /// Returns a <see cref="Configuration.DataType" /> that represents this instance.
        /// </summary>
        /// A <see cref="Configuration.DataType" /> that represents this instance.
        public Configuration.DataType ToDataType()
        {
            return new Configuration.DataType(TypeName, Scale, Precision);
        }
    }
}