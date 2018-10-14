using Acklann.Daterpillar.Configuration;
using System;
using System.Text.RegularExpressions;

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
            TypeMap[BOOL] = "BIT";
            TypeMap[BLOB] = "VARBINARY";
            TypeMap[MEDIUMINT] = "INT";
            TypeMap[TIMESTAMP] = "DATETIME";
        }

        public override string Escape(string objectName)
        {
            return $"[{objectName}]";
        }

        public override string ExpandVariables(string name)
        {
            return Regex.Replace(name, Placeholder.NOW, "GETDATE()");
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a MSSQL data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The MSSQL type name.</returns>
        /// <exception cref="System.ArgumentException">dataType</exception>
        public override string GetTypeName(DataType dataType)
        {
            int scale, precision;
            string type = dataType.Name, name = "";

            switch (type)
            {
                case CHAR:
                case BLOB:
                case VARCHAR:
                    int size = dataType.Scale == 0 ? dataType.Precision : dataType.Scale;
                    name = $"{TypeMap[type]}({(size == 0 ? "MAX" : size.ToString())})";
                    break;

                case DECIMAL:
                    scale = (dataType.Scale == 0 ? 8 : dataType.Scale);
                    precision = (dataType.Precision == 0 ? 2 : dataType.Precision);
                    name = $"{TypeMap[type]}({scale}, {precision})";
                    break;

                default:
                    if (TypeMap.ContainsKey(type)) name = TypeMap[type];
                    else throw new ArgumentException($"Cannot map '{type}' to a data type. Report this issue https://github.com/Ackara/Daterpillar/issues", nameof(dataType));
                    break;
            }

            return name.ToUpper();
        }

        public override string GetActionName(ReferentialAction action)
        {
            switch (action)
            {
                default:
                    return base.GetActionName(action);

                case ReferentialAction.Restrict:
                    return "NO ACTION";
            }
        }
    }
}