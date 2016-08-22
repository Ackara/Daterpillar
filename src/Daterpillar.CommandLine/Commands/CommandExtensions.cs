using Gigobyte.Daterpillar.Arguments;
using Gigobyte.Daterpillar.TextTransformation;
using System.Data;

namespace Gigobyte.Daterpillar.Commands
{
    public static class CommandExtensions
    {
        public static ITemplate AsTemplate(this ConnectionType type)
        {
            switch (type)
            {
                default:
                case ConnectionType.MSSQL:
                    return new MSSQLTemplate();

                case ConnectionType.MySQL:
                    return new MySQLTemplate();

                case ConnectionType.SQLite:
                    return new SQLiteTemplate();
            }
        }

        public static IDbConnection AsConnection(this ConnectionType type)
        {
            return AsConnection(type, string.Empty);
        }

        public static IDbConnection AsConnection(this ConnectionType type, string connectionString)
        {
            switch (type)
            {
                default:
                case ConnectionType.MSSQL:
                    return new System.Data.SqlClient.SqlConnection(connectionString);

                case ConnectionType.MySQL:
                    return new MySql.Data.MySqlClient.MySqlConnection(connectionString);

                case ConnectionType.SQLite:
                    return new System.Data.SQLite.SQLiteConnection(connectionString);
            }
        }
    }
}