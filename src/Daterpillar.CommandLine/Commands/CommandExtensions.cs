using Gigobyte.Daterpillar.Arguments;
using Gigobyte.Daterpillar.TextTransformation;
using System.Data;

namespace Gigobyte.Daterpillar.Commands
{
    public static class CommandExtensions
    {
        public static ITemplate AsTemplate(this SupportedDatabase type)
        {
            switch (type)
            {
                default:
                case SupportedDatabase.MSSQL:
                    return new MSSQLTemplate();

                case SupportedDatabase.MySQL:
                    return new MySQLTemplate();

                case SupportedDatabase.SQLite:
                    return new SQLiteTemplate();
            }
        }

        public static IDbConnection AsConnection(this SupportedDatabase type)
        {
            return AsConnection(type, string.Empty);
        }

        public static IDbConnection AsConnection(this SupportedDatabase type, string connectionString)
        {
            switch (type)
            {
                default:
                case SupportedDatabase.MSSQL:
                    return new System.Data.SqlClient.SqlConnection(connectionString);

                case SupportedDatabase.MySQL:
                    return new MySql.Data.MySqlClient.MySqlConnection(connectionString);

                case SupportedDatabase.SQLite:
                    return new System.Data.SQLite.SQLiteConnection(connectionString);
            }
        }
    }
}