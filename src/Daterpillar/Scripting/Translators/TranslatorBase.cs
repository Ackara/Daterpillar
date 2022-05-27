using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;
using System.Collections.Generic;

namespace Acklann.Daterpillar.Scripting.Translators
{
    /// <summary>
    /// Provides methods for translating SQL name/type to another SQL dialect.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.Translators.ITranslator"/>
    public abstract partial class TranslatorBase : ITranslator
    {
        #region Keys

        /// <summary>
        /// null.
        /// </summary>
        internal const string NULL = "NULL";

        /// <summary>
        /// bool.
        /// </summary>
        internal const string BOOL = "bool";

        /// <summary>
        /// blob.
        /// </summary>
        internal const string BLOB = "blob";

        /// <summary>
        /// char.
        /// </summary>
        internal const string CHAR = "char";

        /// <summary>
        /// text.
        /// </summary>
        internal const string TEXT = "text";

        /// <summary>
        /// varchar.
        /// </summary>
        internal const string VARCHAR = "varchar";

        /// <summary>
        /// int.
        /// </summary>
        internal const string INT = "int";

        /// <summary>
        /// bigInt.
        /// </summary>
        internal const string BIGINT = "bigInt";

        /// <summary>
        /// mediumInt.
        /// </summary>
        internal const string MEDIUMINT = "mediumInt";

        /// <summary>
        /// smallInt.
        /// </summary>
        internal const string SMALLINT = "smallInt";

        /// <summary>
        /// tinyInt.
        /// </summary>
        internal const string TINYINT = "tinyInt";

        /// <summary>
        /// float.
        /// </summary>
        internal const string FLOAT = "float";

        /// <summary>
        /// double.
        /// </summary>
        internal const string DOUBLE = "double";

        /// <summary>
        /// decimal.
        /// </summary>
        internal const string DECIMAL = "decimal";

        /// <summary>
        /// date.
        /// </summary>
        internal const string DATE = "date";

        /// <summary>
        /// time.
        /// </summary>
        internal const string TIME = "time";

        /// <summary>
        /// dateTime.
        /// </summary>
        internal const string DATETIME = "dateTime";

        /// <summary>
        /// timeStamp.
        /// </summary>
        internal const string TIMESTAMP = "timeStamp";

        #endregion Keys

        /// <summary>
        /// Initializes a new instance of the <see cref="TranslatorBase"/> class.
        /// </summary>
        public TranslatorBase()
        {
            TypeMap = new Dictionary<string, string>
            {
                { "bool", "BOOL" },
                { "blob", "BLOB" },
                { "char", "CHAR" },
                { "text", "TEXT" },
                { "varchar", "VARCHAR" },
                { "int", "INT" },
                { "bigInt", "BIGINT" },
                { "mediumInt", "MEDIUMINT" },
                { "smallInt", "SMALLINT" },
                { "tinyInt", "TINYINT" },
                { "float", "FLOAT" },
                { "double", "DOUBLE" },
                { "decimal", "DECIMAL" },
                { "date", "DATE" },
                { "time", "TIME" },
                { "dateTime", "DATETIME" },
                { "timeStamp", "TIMESTAMP" },
            };
        }

        /// <summary>
        /// Gets or sets the type-to-type map.
        /// </summary>
        protected IDictionary<string, string> TypeMap { get; set; }

        /// <summary>
        /// Converts the specified <see cref="SchemaType"/> value to its equivalent string representation.
        /// </summary>
        public static string ConvertToString(SchemaType type)
        {
            switch (type)
            {
                default:
                    return null;

                case SchemaType.BOOL:
                    return BOOL;

                case SchemaType.BLOB:
                    return BLOB;

                case SchemaType.CHAR:
                    return CHAR;

                case SchemaType.TEXT:
                    return TEXT;

                case SchemaType.VARCHAR:
                    return VARCHAR;

                case SchemaType.INT:
                    return INT;

                case SchemaType.BIGINT:
                    return BIGINT;

                case SchemaType.MEDIUMINT:
                    return MEDIUMINT;

                case SchemaType.SMALLINT:
                    return SMALLINT;

                case SchemaType.TINYINT:
                    return TINYINT;

                case SchemaType.FLOAT:
                    return FLOAT;

                case SchemaType.DOUBLE:
                    return DOUBLE;

                case SchemaType.DECIMAL:
                    return DECIMAL;

                case SchemaType.DATE:
                    return DATE;

                case SchemaType.TIME:
                    return TIME;

                case SchemaType.DATETIME:
                    return DATETIME;

                case SchemaType.TIMESTAMP:
                    return TIMESTAMP;
            }
        }

        /// <summary>
        /// Replaces the name of each placeholder variable embedded in the specified string with the
        /// string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">A string containing the names of zero or more environment variables.</param>
        /// <returns></returns>
        public virtual string ExpandVariables(string name) => name;

        /// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>The escaped name.</returns>
		public virtual string Escape(string objectName) => objectName;

        /// <summary>
        /// Converts the <see cref="ReferentialAction"/> value to its equivalent SQL representation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public virtual string ConvertToString(ReferentialAction action)
        {
            switch (action)
            {
                default:
                case ReferentialAction.Cascade:
                    return "CASCADE";

                case ReferentialAction.NoAction:
                    return "NO ACTION";

                case ReferentialAction.Restrict:
                    return "RESTRICT";

                case ReferentialAction.SetNull:
                    return "SET NULL";

                case ReferentialAction.SetDefault:
                    return "SET DEFAULT";
            }
        }

        /// <summary>
        /// Converts the <see cref="DataType"/> value to its equivalent SQL representation.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The type name.</returns>
        public abstract string ConvertToString(DataType dataType);

        /// <summary>
        /// Get default value for the specified <paramref name="typeName"/>.
        /// </summary>
        /// <param name="typeName">The name of the data-type.</param>
        /// <returns>A default value.</returns>
        public virtual string GetDefaultValue(string typeName)
        {
            if (typeName == null) return NULL;

            switch (typeName)
            {
                default: return "''";

                case INT:
                case TINYINT:
                case SMALLINT:
                case MEDIUMINT:
                case BIGINT:
                case FLOAT:
                case DOUBLE:
                case DECIMAL:
                case BOOL: return "0";
            }
        }

        internal struct PlaceholderPattern
        {
            public const string NOW = @"(?i)\$\(now\)";
        }
    }
}