
namespace Acklann.Daterpillar.Configuration
{
	public partial struct DataType
	{
		
		/// <summary>
		/// Converts the specified <see cref="SchemaType" /> value to its equivalent string representation.
		/// </summary>
		public static string ToString(SchemaType type)
		{
			switch (type)
			{
				default:
					return null;
				case SchemaType.BOOL:
					return "bool";
				case SchemaType.BLOB:
					return "blob";
				case SchemaType.CHAR:
					return "char";
				case SchemaType.TEXT:
					return "text";
				case SchemaType.VARCHAR:
					return "varchar";
				case SchemaType.INT:
					return "int";
				case SchemaType.BIGINT:
					return "bigInt";
				case SchemaType.MEDIUMINT:
					return "mediumInt";
				case SchemaType.SMALLINT:
					return "smallInt";
				case SchemaType.TINYINT:
					return "tinyInt";
				case SchemaType.FLOAT:
					return "float";
				case SchemaType.DOUBLE:
					return "double";
				case SchemaType.DECIMAL:
					return "decimal";
				case SchemaType.DATE:
					return "date";
				case SchemaType.TIME:
					return "time";
				case SchemaType.DATETIME:
					return "dateTime";
			}
		}
	}
}