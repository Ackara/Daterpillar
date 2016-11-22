namespace Acklann.Daterpillar.TextTransformation
{
    public sealed class SQLiteTypeNameResolver : TypeNameResolverBase
    {
        public SQLiteTypeNameResolver() : base()
        {
            TypeNames[BOOL] = "boolean";
            TypeNames[BLOB] = "blob";
            TypeNames[CHAR] = "char";
            TypeNames[TEXT] = "text";
            TypeNames[VARCHAR] = "varchar";
            TypeNames[INT] = "integer";
            TypeNames[BIGINT] = "bigInt";
            TypeNames[MEDIUMINT] = "integer";
            TypeNames[SMALLINT] = "integer";
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
                    name = TypeNames[type];
                    break;
            }

            return name.ToUpper();
        }
    }
}