using System;

namespace Ackara.Daterpillar.TypeResolvers
{
    /// <summary>
    /// Provides a method that maps a <see cref="http://static.acklann.com/schema/v2/daterpillar.xsd"/> TypeName to to a MSSQL data type.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class MSSQLTypeResolver : TypeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLTypeResolver"/> class.
        /// </summary>
        public MSSQLTypeResolver()
        {
            TypeMap[BOOL] = "bit";
            TypeMap[BLOB] = "varbinary";
            TypeMap[CHAR] = "char";
            TypeMap[TEXT] = "text";
            TypeMap[VARCHAR] = "varchar";
            TypeMap[INT] = "int";
            TypeMap[BIGINT] = "bigInt";
            TypeMap[MEDIUMINT] = "int";
            TypeMap[SMALLINT] = "smallInt";
            TypeMap[FLOAT] = "float";
            TypeMap[DOUBLE] = "double";
            TypeMap[DECIMAL] = "decimal";
            TypeMap[DATE] = "date";
            TypeMap[TIME] = "time";
            TypeMap[DATETIME] = "dateTime";
            TypeMap[TINYINT] = "tinyInt";
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a MSSQL data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The MSSQL type name.</returns>
        /// <exception cref="System.ArgumentException">dataType</exception>
        public override string GetTypeName(DataType dataType)
        {
            string name = "";
            string type = dataType.Name.ToLower();

            switch (type)
            {
                case CHAR:
                case BLOB:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{TypeMap[type]}({(size == 0 ? "MAX" : size.ToString())})";
                    break;

                case DECIMAL:
                    name = $"{TypeMap[type]}({dataType.Scale}, {dataType.Precision})";
                    break;

                default:
                    if (TypeMap.ContainsKey(type)) name = TypeMap[type];
                    else throw new ArgumentException($"Cannot map '{type}' to a data type. Report this issue https://github.com/Ackara/Daterpillar/issues", nameof(dataType));
                    break;
            }

            return name.ToUpper();
        }
    }
}