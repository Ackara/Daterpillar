using System.Linq;
using System.Collections.Generic;

namespace Ackara.Daterpillar.TypeResolvers
{
	/// <summary>
	/// Defines a method that maps a <see cref="http://static.acklann.com/schema/v2/daterpillar.xsd"/> TypeName to another language's type name.
	/// </summary>
	/// <seealso cref="ITypeResolver" />
	public abstract class TypeResolverBase : ITypeResolver
	{
		#region Keys

		public const string BOOL = "bool";
		public const string BLOB = "blob";
		public const string CHAR = "char";
		public const string TEXT = "text";
		public const string VARCHAR = "varchar";
		public const string INT = "int";
		public const string BIGINT = "bigInt";
		public const string MEDIUMINT = "mediumInt";
		public const string SMALLINT = "smallInt";
		public const string TINYINT = "tinyInt";
		public const string FLOAT = "float";
		public const string DOUBLE = "double";
		public const string DECIMAL = "decimal";
		public const string DATE = "date";
		public const string TIME = "time";
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
