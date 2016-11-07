using System.Collections.Generic;

namespace Gigobyte.Daterpillar.TextTransformation
{
	public abstract class TypeNameResolverBase : ITypeNameResolver
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
		public const string FLOAT = "float";
		public const string DOUBLE = "double";
		public const string DECIMAL = "decimal";
		public const string DATE = "date";
		public const string TIME = "time";
		public const string DATETIME = "datetime";

		#endregion Keys

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeNameResolverBase"/> class.
		/// </summary>
		public TypeNameResolverBase()
		{
			TypeNames = new Dictionary<string, string>();
		
			TypeNames.Add("bool", "bool");
			TypeNames.Add("blob", "blob");
			TypeNames.Add("char", "char");
			TypeNames.Add("text", "text");
			TypeNames.Add("varchar", "varchar");
			TypeNames.Add("int", "int");
			TypeNames.Add("bigint", "bigint");
			TypeNames.Add("mediumint", "mediumint");
			TypeNames.Add("smallint", "smallint");
			TypeNames.Add("float", "float");
			TypeNames.Add("double", "double");
			TypeNames.Add("decimal", "decimal");
			TypeNames.Add("date", "date");
			TypeNames.Add("time", "time");
			TypeNames.Add("datetime", "datetime");
		}

		/// <summary>
		/// 
		/// </summary>
		public abstract string GetName(DataType dataType);

		/// <summary>
		/// 
		/// </summary>
		protected IDictionary<string, string> TypeNames { get; set; }
	}
}
