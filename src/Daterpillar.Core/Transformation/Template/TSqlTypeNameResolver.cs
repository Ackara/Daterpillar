namespace Gigobyte.Daterpillar.Transformation.Template
{
    public sealed class TSqlTypeNameResolver : TypeNameResolverBase
    {
        public TSqlTypeNameResolver() : base()
        {
            TypeNames[BOOL] = "bit";
            TypeNames[BLOB] = "varbinary";
            TypeNames[CHAR] = "char";
            TypeNames[TEXT] = "text";
            TypeNames[VARCHAR] = "varchar";
            TypeNames[INT] = "int";
            TypeNames[BIGINT] = "bigInt";
            TypeNames[MEDIUMINT] = "int";
            TypeNames[SMALLINT] = "smallInt";
            TypeNames[FLOAT] = "float";
            TypeNames[DOUBLE] = "float";
            TypeNames[DECIMAL] = "decimal";
            TypeNames[DATE] = "date";
            TypeNames[TIME] = "time";
            TypeNames[DATETIME] = "dateTime";
        }

        public override string GetName(DataType dataType)
        {
            string name = "";
            string type = dataType.Name;

            switch (type)
            {
                case CHAR:
                case BLOB:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{TypeNames[type]}({size})";
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