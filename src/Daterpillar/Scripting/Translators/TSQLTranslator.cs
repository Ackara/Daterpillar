using Acklann.Daterpillar.Annotations;
using Acklann.Daterpillar.Modeling;
using System;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Scripting.Translators
{
    /// <summary>
    /// Provides methods for converting SQL name/type to it TSQL equivalent.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.Translators.TranslatorBase" />
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

        /// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>
        /// The escaped name.
        /// </returns>
        public override string Escape(string objectName)
        {
            return $"[{objectName}]";
        }

        /// <summary>
        /// Replaces the name of each placeholder variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">A string containing the names of zero or more environment variables.</param>
        /// <returns></returns>
        public override string ExpandVariables(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            else return Regex.Replace(name, PlaceholderPattern.NOW, "GETDATE()");
        }

        /// <summary>
        /// Converts the <see cref="Acklann.Daterpillar.Modeling.DataType" /> value to its equivalent TSQL representation.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The <see cref="DataType"/> as a string.</returns>
        /// <exception cref="ArgumentException">Could not map <paramref name="dataType"/></exception>
        public override string ConvertToString(DataType dataType)
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

        /// <summary>
        /// Converts the <see cref="Acklann.Daterpillar.ReferentialAction" /> value to its equivalent TSQL representation.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>The <see cref="ReferentialAction"/> as a string.</returns>
        public override string ConvertToString(ReferentialAction action)
        {
            switch (action)
            {
                default:
                    return base.ConvertToString(action);

                case ReferentialAction.Restrict:
                    return "NO ACTION";
            }
        }
    }
}