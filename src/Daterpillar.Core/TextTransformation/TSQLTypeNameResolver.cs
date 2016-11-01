namespace Gigobyte.Daterpillar.TextTransformation
{
    public class TSQLTypeNameResolver : TypeNameResolverBase
    {
        public TSQLTypeNameResolver()
        {
            TypeNames[BOOL] = "bit";
            TypeNames[BLOB] = "varbinary";
            TypeNames[CHAR] = "char";
            TypeNames[TEXT] = "text";
            TypeNames[VARCHAR] = "varchar";
            TypeNames[INT] = "int";
            TypeNames[BIGINT] = "bigInt";
            TypeNames[MEDIUMINT] = "int";
            TypeNames[SMALLINT] = "smallint";
            TypeNames[FLOAT] = "float";
            TypeNames[DOUBLE] = "double";
            TypeNames[DECIMAL] = "decimal";
            TypeNames[DATE] = "date";
            TypeNames[TIME] = "time";
            TypeNames[DATETIME] = "dateTime";
        }

        public override string GetName(DataType dataType)
        {
            string name = "";
            string type = dataType.Name;

            switch (type.ToLower())
            {
                case CHAR:
                case BLOB:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{TypeNames[type]}({(size == 0 ? "MAX" : size.ToString())})";
                    break;

                case DECIMAL:
                    name = $"{TypeNames[type]}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    name = TypeNames[type];
                    break;
            }

            return name.ToUpper();
        }
    }
}