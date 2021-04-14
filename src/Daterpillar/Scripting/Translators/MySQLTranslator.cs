using Acklann.Daterpillar.Serialization;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Scripting.Translators
{
    /// <summary>
    /// Provides methods for converting SQL name/type to it MySQL equivalent.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.Translators.TranslatorBase" />
    public class MySQLTranslator : TranslatorBase
    {
        /// <summary>
        /// Escapes the specified object name.
        /// </summary>
        /// <param name="objectName">Name of the object.</param>
        /// <returns>
        /// The escaped name.
        /// </returns>
        public override string Escape(string objectName) => $"`{objectName}`";

        /// <summary>
        /// Replaces the name of each placeholder variable embedded in the specified string with the string equivalent of the value of the variable, then returns the resulting string.
        /// </summary>
        /// <param name="name">A string containing the names of zero or more environment variables.</param>
        /// <returns></returns>
        public override string ExpandVariables(string name)
        {
            if (string.IsNullOrEmpty(name)) return string.Empty;
            else return Regex.Replace(name, PlaceholderPattern.NOW, "CURRENT_TIMESTAMP");
        }

        /// <summary>
        /// Converts the <see cref="DataType" /> value to its equivalent MySQL equivalent.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>
        /// The type name.
        /// </returns>
        public override string ConvertToString(DataType dataType)
        {
            int scale, precision;
            string typeName = dataType.Name;

            switch (typeName)
            {
                case CHAR:
                    scale = (dataType.Scale == 0 ? 1 : dataType.Scale);
                    typeName = $"{typeName}({scale})";
                    break;

                case VARCHAR:
                    scale = (dataType.Scale == 0 ? 255 : dataType.Scale);
                    typeName = $"{typeName}({scale})";
                    break;

                case DECIMAL:
                    scale = (dataType.Scale == 0 ? 8 : dataType.Scale);
                    precision = (dataType.Precision == 0 ? 2 : dataType.Precision);

                    typeName = $"{typeName}({scale}, {precision})";
                    break;

                default:
                    typeName = TypeMap[typeName];
                    break;
            }

            return typeName.ToUpper();
        }
    }
}