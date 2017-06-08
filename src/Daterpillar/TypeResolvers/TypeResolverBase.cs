using System.Collections.Generic;

namespace Acklann.Daterpillar.TypeResolvers
{
	/// <summary>
	/// Defines a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to another language's type name.
	/// </summary>
	/// <seealso cref="ITypeResolver" />
	public abstract class TypeResolverBase : ITypeResolver
	{
		#region Keys

		/// <summary>bool.</summary>
		public const string BOOL = "bool";
		/// <summary>blob.</summary>
		public const string BLOB = "blob";
		/// <summary>char.</summary>
		public const string CHAR = "char";
		/// <summary>text.</summary>
		public const string TEXT = "text";
		/// <summary>varchar.</summary>
		public const string VARCHAR = "varchar";
		/// <summary>int.</summary>
		public const string INT = "int";
		/// <summary>bigInt.</summary>
		public const string BIGINT = "bigInt";
		/// <summary>mediumInt.</summary>
		public const string MEDIUMINT = "mediumInt";
		/// <summary>smallInt.</summary>
		public const string SMALLINT = "smallInt";
		/// <summary>tinyInt.</summary>
		public const string TINYINT = "tinyInt";
		/// <summary>float.</summary>
		public const string FLOAT = "float";
		/// <summary>double.</summary>
		public const string DOUBLE = "double";
		/// <summary>decimal.</summary>
		public const string DECIMAL = "decimal";
		/// <summary>date.</summary>
		public const string DATE = "date";
		/// <summary>time.</summary>
		public const string TIME = "time";
		/// <summary>dateTime.</summary>
		public const string DATETIME = "dateTime";

		#endregion Keys

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeResolverBase"/> class.
		/// </summary>
		public TypeResolverBase()
		{
			TypeMap = new Dictionary<string, string>
			{
				{ "bool", "bool" },
				{ "blob", "blob" },
				{ "char", "char" },
				{ "text", "text" },
				{ "varchar", "varchar" },
				{ "int", "int" },
				{ "bigint", "bigint" },
				{ "mediumint", "mediumint" },
				{ "smallint", "smallint" },
				{ "tinyint", "tinyint" },
				{ "float", "float" },
				{ "double", "double" },
				{ "decimal", "decimal" },
				{ "date", "date" },
				{ "time", "time" },
				{ "datetime", "datetime" },
			};
		}

		/// <summary>
		/// Maps the specified <see cref="DataType"/>.
		/// </summary>
		/// <param name="dataType">Type of the data.</param>
		/// <returns>The type name.</returns>
		public abstract string GetTypeName(DataType dataType);

		/// <summary>
		/// Gets or sets the type-to-type map.
		/// </summary>
		protected IDictionary<string, string> TypeMap { get; set; }
	}
}
