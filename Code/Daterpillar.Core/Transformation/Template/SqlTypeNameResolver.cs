namespace Ackara.Daterpillar.Transformation.Template
{
    public class SqlTypeNameResolver : TypeNameResolverBase
    {
        public SqlTypeNameResolver() : base()
        {
        }

        public override string GetName(DataType dataType)
        {
            string name = "";
            string type = dataType.Name;

            switch (type)
            {
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