
using System.Collections.Generic;
using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
	/// <summary>
	/// Defines a method that maps a https://raw.githubusercontent.com/Ackara/Daterpillar/master/src/daterpillar.xsd TypeName to another language's type name.
	/// </summary>
	/// <seealso cref="ITypeResolver" />
	public abstract partial class TypeResolverBase : ITypeResolver
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
		internal const string BIGINT = "bigInt";
		/// <summary>mediumInt.</summary>
		internal const string MEDIUMINT = "mediumInt";
		/// <summary>smallInt.</summary>
		internal const string SMALLINT = "smallInt";
		/// <summary>tinyInt.</summary>
		internal const string TINYINT = "tinyInt";
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
		internal const string DATETIME = "dateTime";
		/// <summary>timeStamp.</summary>
		internal const string TIMESTAMP = "timeStamp";

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
	}
}