﻿using Acklann.Daterpillar.Configuration;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
    /// <summary>
    /// Provides a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to to a MySQL data type.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class MySQLTypeResolver : TypeResolverBase
    {
        public MySQLTypeResolver()
        {
        }

        public override string Escape(string objectName) => $"`{objectName}`";

        public override string ExpandVariables(string name)
        {
            return Regex.Replace(name, Placeholder.NOW, "CURRENT_TIMESTAMP");
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a MySQL data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The MySQL type name.</returns>
        public override string GetTypeName(DataType dataType)
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