
using System.Collections.Generic;
using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
	/// <summary>
	/// Defines a method that maps a https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/daterpillar.xsd TypeName to another language's type name.
	/// </summary>
	/// <seealso cref="ITypeResolver" />
	public abstract class TypeResolverBase : ITypeResolver
	{
		#region Keys

		/// <summary>bool.</summary>
		internal const string BOOL = "bool";
		/// <summary>blob.</summary>
		internal const string BLOB = "blob";
		/// <summary>char.</summary>
		internal const string CHAR = "char";
		/// <summary>text.</summary>
		internal const string TEXT = "text";
		/// <summary>varchar.</summary>
		internal const string VARCHAR = "varchar";
		/// <summary>int.</summary>
		internal const string INT = "int";
		/// <summary>bigInt.</summary>
		internal const string BIGINT = "bigint";
		/// <summary>mediumInt.</summary>
		internal const string MEDIUMINT = "mediumint";
		/// <summary>smallInt.</summary>
		internal const string SMALLINT = "smallint";
		/// <summary>tinyInt.</summary>
		internal const string TINYINT = "tinyint";
		/// <summary>float.</summary>
		internal const string FLOAT = "float";
		/// <summary>double.</summary>
		internal const string DOUBLE = "double";
		/// <summary>decimal.</summary>
		internal const string DECIMAL = "decimal";
		/// <summary>date.</summary>
		internal const string DATE = "date";
		/// <summary>time.</summary>
		internal const string TIME = "time";
		/// <summary>dateTime.</summary>
		internal const string DATETIME = "datetime";
		/// <summary>timeStamp.</summary>
		internal const string TIMESTAMP = "timestamp";

		#endregion Keys

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeResolverBase"/> class.
		/// </summary>
		public TypeResolverBase()
		{
			TypeMap = new Dictionary<string, string>
			{
				{ "bool", "BOOL" },
				{ "blob", "BLOB" },
				{ "char", "CHAR" },
				{ "text", "TEXT" },
				{ "varchar", "VARCHAR" },
				{ "int", "INT" },
				{ "bigint", "BIGINT" },
				{ "mediumint", "MEDIUMINT" },
				{ "smallint", "SMALLINT" },
				{ "tinyint", "TINYINT" },
				{ "float", "FLOAT" },
				{ "double", "DOUBLE" },
				{ "decimal", "DECIMAL" },
				{ "date", "DATE" },
				{ "time", "TIME" },
				{ "datetime", "DATETIME" },
				{ "timestamp", "TIMESTAMP" },
			};
		}

		/// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>The escaped name.</returns>
		public virtual string Escape(string objectName) => objectName;

		/// <summary>
		/// Maps the specified <see cref="DataType"/>.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>The type name.</returns>
		public abstract string GetTypeName(DataType dataType);

		public virtual string GetActionName(ReferentialAction action)
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
		/// Gets or sets the type-to-type map.
		/// </summary>
		protected IDictionary<string, string> TypeMap { get; set; }
	}
}