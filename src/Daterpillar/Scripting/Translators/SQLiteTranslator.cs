using Acklann.Daterpillar.Serialization;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Scripting.Translators
{
    /// <summary>
    /// Provides methods for converting SQL name/type to it SQLite equivalent.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.TypeResolvers.TypeResolverBase" />
    public class SQLiteTranslator : TranslatorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTranslator"/> class.
        /// </summary>
        public SQLiteTranslator() : base()
        {
            TypeMap[BOOL] = "BOOLEAN";
            TypeMap[INT] = "INTEGER";
            TypeMap[MEDIUMINT] = "INTEGER";
            TypeMap[SMALLINT] = "INTEGER";
            TypeMap[TINYINT] = "INTEGER";
            TypeMap[TIMESTAMP] = "TEXT";
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
            return Regex.Replace(name, PlaceholderPattern.NOW, "''");
        }

        /// <summary>
        /// Converts the <see cref="DataType" /> value to its equivalent SQLite equivalent.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>A SQLite data type.</returns>
        public override string ConvertToString(DataType dataType)
        {
            int s, p;
            string name = "";
            string type = dataType.Name;

            switch (type)
            {
                case CHAR:
                    s = (dataType.Scale == 0 ? 1 : dataType.Scale);
                    name = $"{type}({s})";
                    break;

                case VARCHAR:
                    s = dataType.Scale == 0 ? 255 : dataType.Scale;
                    name = $"{type}({s})";
                    break;

                case DECIMAL:
                    s = (dataType.Scale == 0 ? 8 : dataType.Scale);
                    p = (dataType.Precision == 0 ? 2 : dataType.Precision);
                    name = $"{type}({s},{p})";
                    break;

                default:
                    name = TypeMap[type];
                    break;
            }

            return name.ToUpper();
        }
    }
}