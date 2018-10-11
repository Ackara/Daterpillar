using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation.Resolvers
{
    /// <summary>
    /// Provides a method that maps a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to to a SQLite data type.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.TypeResolvers.TypeResolverBase" />
    public class SQLiteTypeResolver : TypeResolverBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteTypeResolver"/> class.
        /// </summary>
        public SQLiteTypeResolver() : base()
        {
            TypeMap[BOOL] = "BOOLEAN";
            TypeMap[INT] = "INTEGER";
            TypeMap[MEDIUMINT] = "INTEGER";
            TypeMap[SMALLINT] = "INTEGER";
            TypeMap[TINYINT] = "INTEGER";
            TypeMap[TIMESTAMP] = "TEXT";
        }

        public override string Escape(string objectName)
        {
            return $"[{objectName}]";
        }

        /// <summary>
        /// Maps the specified <see cref="T:Ackara.Daterpillar.DataType" /> to a SQLite data type.
        /// </summary>
        /// <param name="dataType">Type of the data.</param>
        /// <returns>The SQLite type name.</returns>
        public override string GetTypeName(DataType dataType)
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