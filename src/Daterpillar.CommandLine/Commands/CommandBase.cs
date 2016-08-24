using System.Data;

namespace Gigobyte.Daterpillar.Commands
{
    public abstract class CommandBase : ICommand
    {
        public abstract int Execute(object args);

        protected IDbConnection GetConnection(SupportedDatabase type)
        {
            return GetConnection(type, string.Empty);
        }

        protected IDbConnection GetConnection(SupportedDatabase type, string connectionString)
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