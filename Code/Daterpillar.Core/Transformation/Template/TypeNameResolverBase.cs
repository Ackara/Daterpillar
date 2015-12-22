using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Transformation.Template
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
		public const string BIGINT = "bigInt";
		public const string MEDIUMINT = "mediumInt";
		public const string SMALLINT = "smallInt";
		public const string FLOAT = "float";
		public const string DOUBLE = "double";
		public const string DECIMAL = "decimal";
		public const string DATE = "date";
		public const string TIME = "time";
		public const string DATETIME = "dateTime";

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
			TypeNames.Add("bigInt", "bigInt");
			TypeNames.Add("mediumInt", "mediumInt");
			TypeNames.Add("smallInt", "smallInt");
			TypeNames.Add("float", "float");
			TypeNames.Add("double", "double");
			TypeNames.Add("decimal", "decimal");
			TypeNames.Add("date", "date");
			TypeNames.Add("time", "time");
			TypeNames.Add("dateTime", "dateTime");
		}

		protected IDictionary<string, string> TypeNames { get; set; }

		public abstract string GetName(DataType dataType);
	}
}
