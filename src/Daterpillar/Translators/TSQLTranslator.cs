using Acklann.Daterpillar.Configuration;
using System;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Translators
{
    /// <summary>
    /// Provides a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to to a MSSQL data type.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class TSQLTranslator : TranslatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TSQLTranslator"/> class.
        /// </summary>
        public TSQLTranslator()
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
            if (string.IsNullOrEmpty(name)) return string.Empty;
            else return Regex.Replace(name, Placeholder.NOW, "GETDATE()");
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
            string type = dataType.Name, name = "", temp;

            switch (type)
            {
                case CHAR:
                    scale = (dataType.Scale == 0 ? 1 : dataType.Scale);
                    name = $"{TypeMap[type]}({scale})";
                    break;

                case BLOB:
                case VARCHAR:
                    if (dataType.Scale == 0) temp = "255";
                    else if (dataType.Scale == int.MaxValue) temp = "MAX";
                    else temp = dataType.Scale.ToString();

                    name = $"{TypeMap[type]}({temp})";
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