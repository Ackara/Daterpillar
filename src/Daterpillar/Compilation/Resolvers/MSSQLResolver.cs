using Acklann.Daterpillar.Configuration;
using System;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
    /// <summary>
    /// Provides a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to to a MSSQL data type.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class MSSQLResolver : TypeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLResolver"/> class.
        /// </summary>
        public MSSQLResolver()
        {
            TypeMap[BOOL.ToLower()] = "bit";
            TypeMap[BLOB.ToLower()] = "varbinary";
            TypeMap[MEDIUMINT.ToLower()] = "int";
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