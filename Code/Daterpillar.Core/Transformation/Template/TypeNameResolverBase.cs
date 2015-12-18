using System.Collections.Generic;

namespace Ackara.Daterpillar.Transformation.Template
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
			TypeNameDictionary = new Dictionary<string, string>();
		
			TypeNameDictionary.Add("bool", "bool");
			TypeNameDictionary.Add("blob", "blob");
			TypeNameDictionary.Add("char", "char");
			TypeNameDictionary.Add("text", "text");
			TypeNameDictionary.Add("varchar", "varchar");
			TypeNameDictionary.Add("int", "int");
			TypeNameDictionary.Add("bigInt", "bigInt");
			TypeNameDictionary.Add("mediumInt", "mediumInt");
			TypeNameDictionary.Add("smallInt", "smallInt");
			TypeNameDictionary.Add("float", "float");
			TypeNameDictionary.Add("double", "double");
			TypeNameDictionary.Add("decimal", "decimal");
			TypeNameDictionary.Add("date", "date");
			TypeNameDictionary.Add("time", "time");
			TypeNameDictionary.Add("dateTime", "dateTime");
		}

		protected IDictionary<string, string> TypeNameDictionary { get; set; }

		public abstract string GetName(DataType dataType);
	}
}
