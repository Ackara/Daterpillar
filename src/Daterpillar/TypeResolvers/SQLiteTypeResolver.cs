namespace Ackara.Daterpillar.TypeResolvers
{
    /// <summary>
    /// Provides a method that maps a <see cref="http://static.acklann.com/schema/v2/daterpillar.xsd"/> TypeName to to a SQLite data type.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class SQLiteTypeResolver : TypeResolverBase
    {
        public SQLiteTypeResolver() : base()
        {
            TypeMap[BOOL] = "boolean";
            TypeMap[BLOB] = "blob";
            TypeMap[CHAR] = "char";
            TypeMap[TEXT] = "text";
            TypeMap[VARCHAR] = "varchar";
            TypeMap[INT] = "integer";
            TypeMap[BIGINT] = "bigInt";
            TypeMap[MEDIUMINT] = "integer";
            TypeMap[SMALLINT] = "integer";
            TypeMap[TINYINT] = "integer";
            TypeMap[FLOAT] = "float";
            TypeMap[DOUBLE] = "double";
            TypeMap[DECIMAL] = "decimal";
            TypeMap[DATE] = "date";
            TypeMap[TIME] = "time";
            TypeMap[DATETIME] = "dateTime";
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a SQLite data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The SQLite type name.</returns>
        public override string GetTypeName(DataType dataType)
        {
            string name = "";
            string type = dataType.Name.ToLower();

            switch (type)
            {
                case CHAR:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{type}({size})";
                    break;

                case DECIMAL:
                    name = $"{type}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    name = TypeMap[type];
                    break;
            }

            return name.ToUpper();
        }
    }
}