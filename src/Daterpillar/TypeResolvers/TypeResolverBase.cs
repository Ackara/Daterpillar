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
		public const string BIGINT = "bigint";
		public const string MEDIUMINT = "mediumint";
		public const string SMALLINT = "smallint";
		public const string TINYINT = "tinyint";
		public const string FLOAT = "float";
		public const string DOUBLE = "double";
		public const string DECIMAL = "decimal";
		public const string DATE = "date";
		public const string TIME = "time";
		public const string DATETIME = "datetime";

		#endregion Keys

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeResolverBase"/> class.
		/// </summary>
		public TypeResolverBase()
		{
			TypeMap = new Dictionary<string, string>
			{
				{ BOOL, "bool" },
				{ BLOB, "blob" },
				{ CHAR, "char" },
				{ TEXT, "text" },
				{ VARCHAR, "varchar" },
				{ INT, "int" },
				{ BIGINT, "bigint" },
				{ MEDIUMINT, "mediumint" },
				{ SMALLINT, "smallint" },
				{ TINYINT, "tinyint" },
				{ FLOAT, "float" },
				{ DOUBLE, "double" },
				{ DECIMAL, "decimal" },
				{ DATE, "date" },
				{ TIME, "time" },
				{ DATETIME, "datetime" },
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
