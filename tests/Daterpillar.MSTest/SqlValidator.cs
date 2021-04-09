using System;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar
{
    internal static class SqlValidator
    {
        public static bool TryExecute(this IDbConnection connection, string sql, out string errorMsg)
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
********** !!! SCRIPT FAILED !!! **********
{ex.Message}
*****************************************

".TrimStart();
            }

            return false;
        }

        public static IDbConnection ClearDatabase(Language connectionType)
        {
            switch (connectionType)
            {
                case Language.MySQL:
                    return ClearMySqlDatabase();

                case Language.TSQL:
                    return ClearSqlServerDatabase();

                case Language.SQLite:
                    return ClearSQLiteDatabase();

                default: throw new System.NotImplementedException();
            }
        }

        #region Backing Members

        private static System.Data.SqlClient.SqlConnection CreateSqlServerConnection()
        {
            return new System.Data.SqlClient.SqlConnection(TestData.GetValue("tsql"));
        }

        private static System.Data.SqlClient.SqlConnection ClearSqlServerDatabase()
        {
            System.Data.SqlClient.SqlConnection connection = RemoveSqlServerDatabase();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE [{nameof(Daterpillar)}];";
                command.ExecuteNonQuery();
                connection.ChangeDatabase(nameof(Daterpillar));
            }

            return connection;
        }

        private static System.Data.SqlClient.SqlConnection RemoveSqlServerDatabase()
        {
            System.Data.SqlClient.SqlConnection connection = CreateSqlServerConnection();
            IDbCommand command = null;

            try
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                connection.ChangeDatabase("master");

                command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE [{nameof(Daterpillar)}];";
                command.ExecuteNonQuery();
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (ex.Number != 3701) throw ex;
            }
            finally { command?.Dispose(); }

            return connection;
        }

        private static MySql.Data.MySqlClient.MySqlConnection CreateMySqlConnection()
        {
            return new MySql.Data.MySqlClient.MySqlConnection(TestData.GetValue("mysql"));
        }

        private static MySql.Data.MySqlClient.MySqlConnection ClearMySqlDatabase()
        {
            MySql.Data.MySqlClient.MySqlConnection connection = RemoveMySqlDatabase();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = $"CREATE DATABASE `{nameof(Daterpillar)}`;";
                command.ExecuteNonQuery();
                connection.ChangeDatabase(nameof(Daterpillar));
            }

            return connection;
        }

        private static MySql.Data.MySqlClient.MySqlConnection RemoveMySqlDatabase()
        {
            MySql.Data.MySqlClient.MySqlConnection connection = CreateMySqlConnection();

            IDbCommand command = null;
            if (connection.State != ConnectionState.Open) connection.Open();
            connection.ChangeDatabase("information_schema");

            try
            {
                command = connection.CreateCommand();
                command.CommandText = $"DROP DATABASE `{nameof(Daterpillar)}`";
                command.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                if (ex.Number != 1008) throw ex;
            }
            finally { command?.Dispose(); }

            return connection;
        }

        private static System.Data.SQLite.SQLiteConnection CreateSQLiteConnection()
        {
            return new System.Data.SQLite.SQLiteConnection(new System.Data.SQLite.SQLiteConnectionStringBuilder
            {
                DataSource = _sqliteFilePath
            }.ToString());
        }

        private static System.Data.SQLite.SQLiteConnection ClearSQLiteDatabase()
        {
            if (File.Exists(_sqliteFilePath)) File.Delete(_sqliteFilePath);
            string folder = Path.GetDirectoryName(_sqliteFilePath);
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            System.Data.SQLite.SQLiteConnection.CreateFile(_sqliteFilePath);

            return CreateSQLiteConnection();
        }

        private static readonly string _sqliteFilePath = Path.Combine(Path.GetTempPath(), "daterpillar-mstest.db3");

        #endregion Backing Members
    }
}