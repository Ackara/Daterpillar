namespace Acklann.Daterpillar.TextTransformation
{
    public class MSSQLTypeNameResolver : TypeNameResolverBase
    {
        public MSSQLTypeNameResolver()
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
                case BLOB:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{TypeNames[type]}({(size == 0 ? "MAX" : size.ToString())})";
                    break;

                case DECIMAL:
                    name = $"{TypeNames[type]}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    if (TypeNames.ContainsKey(type)) name = TypeNames[type];
                    else throw new System.ArgumentException($"Cannot map '{type}' to a data type. Report this issue https://github.com/Ackara/Daterpillar/issues", nameof(dataType));
                    break;
            }

            return name.ToUpper();
        }
    }
}