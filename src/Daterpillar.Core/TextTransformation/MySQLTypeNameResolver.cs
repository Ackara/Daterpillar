namespace Gigobyte.Daterpillar.TextTransformation
{
    public sealed class MySQLTypeNameResolver : TypeNameResolverBase
    {
        public MySQLTypeNameResolver()
        {
        }

        public override string GetName(DataType dataType)
        {
            string typeName = dataType.Name.ToLower();
            switch (typeName)
            {
                case CHAR:
                case VARCHAR:
                    typeName = $"{typeName}({dataType.Scale})";
                    break;

                case DECIMAL:
                    typeName = $"{typeName}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    typeName = TypeNames[typeName];
                    break;
            }

            return typeName.ToUpper();
        }
    }
}