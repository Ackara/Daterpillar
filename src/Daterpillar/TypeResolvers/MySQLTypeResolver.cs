namespace Ackara.Daterpillar.TypeResolvers
{
    /// <summary>
    /// Provides a method that maps a <see cref="http://static.acklann.com/schema/v2/daterpillar.xsd"/> TypeName to to a MySQL data type.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class MySQLTypeResolver : TypeResolverBase
    {
        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a MySQL data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The MySQL type name.</returns>
        public override string GetTypeName(DataType dataType)
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
                    typeName = TypeMap[typeName];
                    break;
            }

            return typeName.ToUpper();
        }
    }
}