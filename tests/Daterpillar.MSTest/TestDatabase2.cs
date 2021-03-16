using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar
{
    internal static class TestDatabase2
    {
        public static IDbConnection CreateConnection(Language type, bool useMaster = false)
        {
            IDbConnection connection = null;
            string connectionString = TestData.GetValue(type.ToString(), null);

            switch (type)
            {
                case Language.TSQL:
                    connection = new System.Data.SqlClient.SqlConnection(connectionString);
                    if (useMaster) connection.ChangeDatabase("master");
                    break;

                case Language.MySQL:
                    connection = new MySql.Data.MySqlClient.MySqlConnection(connectionString);
                    if (useMaster) connection.ChangeDatabase("master");
                    break;

                case Language.SQLite:
                    if (!File.Exists(_sqliteFilePath)) System.Data.SQLite.SQLiteConnection.CreateFile(_sqliteFilePath);
                    connection = new System.Data.SQLite.SQLiteConnection(new System.Data.SQLite.SQLiteConnectionStringBuilder() { DataSource = _sqliteFilePath }.ConnectionString);
                    break;
            }

            return connection;
        }

        public static bool TryExecute2(this IDbConnection connection, string sql, out string errorMsg)
        {
            errorMsg = null;
            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                foreach (string batch in new Regex(@"(?i)\r?\nGO\r?\n").Split(sql))
                    if (string.IsNullOrEmpty(batch) == false)
                        using (IDbCommand command = connection.CreateCommand())
                        {
                            command.CommandText = batch;
                            command.ExecuteNonQuery();
                        }

                return true;
            }
            catch (Exception ex)
            {
                errorMsg = $@"
********** !!! TEST FAILED !!! **********
{ex.ToString()}
*****************************************

".TrimStart();
            }

            return false;
        }

        public static void ClearSchema(this IDbConnection connection, Language dialect)
        {
            string databaseName = connection.Database;
            if (connection.State != ConnectionState.Open) connection.Open();

            bool success; string error;

            switch (dialect)
            {
                case Language.TSQL:
                    connection.ChangeDatabase("master");
                    TryExecute2(connection, $"DROP DATABASE [{databaseName}]", out error);
                    success = TryExecute2(connection, $"CREATE DATABASE [{databaseName}]", out error);
                    if (success == false) throw new System.SystemException(error);

                    connection.ChangeDatabase(databaseName);
                    break;

                case Language.MySQL:
                    connection.ChangeDatabase("mysql");
                    TryExecute2(connection, $"DROP DATABASE `{databaseName}`", out error);
                    success = TryExecute2(connection, $"CREATE DATABASE `{databaseName}`", out error);
                    if (success == false) throw new System.SystemException(error);

                    connection.ChangeDatabase(databaseName);
                    break;

                case Language.SQLite:
                    connection.Close();
                    if (File.Exists(_sqliteFilePath)) File.Delete(_sqliteFilePath);
                    System.Data.SQLite.SQLiteConnection.CreateFile(_sqliteFilePath);

                    break;
            }
        }

        private static readonly string _sqliteFilePath = Path.Combine(Path.GetTempPath(), "daterpillar-mstest.db3");
    }
}