using Acklann.Daterpillar.Modeling;
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

        public static IDbConnection CreateSchemaTable(Type type, Language dialect)
        {
            using (var stream = new MemoryStream())
            using (Scripting.Writers.DDLWriter writer = new Scripting.Writers.SqlWriterFactory().CreateInstance(dialect, stream))
            {
                Table table = SchemaFactory.CreateFrom(type);
                writer.Create(table);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);

                string script = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                IDbConnection connection = CreateConnection(dialect);
                connection.Open();
                try { connection.ChangeDatabase(nameof(Daterpillar)); } catch (NotImplementedException) { }

                bool failed = !TryExecute(connection, script, out string error);
                if (failed) throw new System.Exception(error);

                return connection;
            }
        }

        public static IDbConnection CreateConnection(Language connectionType)
        {
            switch (connectionType)
            {
                case Language.MySQL: return CreateMySqlConnection();
                case Language.SQLite: return CreateSQLiteConnection();
                case Language.TSQL: return CreateSqlServerConnection();

                default: throw new NotImplementedException();
            }
        }

        public static void CreateDatabase(params Language[] connectionTypes)
        {
            var factory = new Scripting.Writers.SqlWriterFactory();
            if (connectionTypes.Length == 0) connectionTypes = new Language[] { Language.MySQL, Language.TSQL, Language.SQLite };

            foreach (Language lang in connectionTypes)
            {
                using (IDbConnection connection = ClearDatabase(lang))
                using (var stream = new MemoryStream())
                using (var writer = factory.CreateInstance(lang, stream))
                {
                    Schema schema = SchemaFactory.CreateFrom(typeof(SqlValidator).Assembly);
                    writer.Create(schema);
                    writer.Flush();
                    stream.Seek(0, SeekOrigin.Begin);

                    try { connection.ChangeDatabase(nameof(Daterpillar)); }
                    catch (System.Data.Common.DbException) { }
                    catch (NotImplementedException) { }
                    string sql = System.Text.Encoding.UTF8.GetString(stream.ToArray());
                    bool failed = !TryExecute(connection, sql, out string error);
                    if (failed) throw new Exception(error);
                }
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