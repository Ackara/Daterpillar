using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using System;
using System.Data;

namespace Tests.Daterpillar.Utilities
{
    public static class DatabaseBuilder
    {
        public static bool TryResetDatabase(string connectionName, ITemplate template)
        {
            var settings = System.Configuration.ConfigurationManager.ConnectionStrings[connectionName];
            var connection = (IDbConnection)Activator.CreateInstance(Type.GetType(settings.ProviderName));
            var schema = Schema.Load(Test.Data.GetFile(Test.File.DataSoftXDDL).OpenRead());

            return TryResetDatabase(connection, schema, template);
        }

        public static bool TryResetDatabase(IDbConnection connection, ITemplate template)
        {
            var schema = Schema.Load(Test.Data.GetFile(Test.File.DataSoftXDDL).OpenRead());
            return TryResetDatabase(connection, schema, template);
        }

        public static bool TryResetDatabase(IDbConnection connection, Schema schema, ITemplate template)
        {
            try
            {
                TruncateDatabase(connection, schema);
                CreateSchema(connection, schema, template);
                return true;
            }
            catch (System.Data.Common.DbException ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR: {ex.ErrorCode}");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
            finally { connection?.Dispose(); }
        }

        internal static void TruncateDatabase(IDbConnection connection, Schema schema)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            using (var commmand = connection.CreateCommand())
            {
                for (int i = schema.Tables.Count - 1; i >= 0; i--)
                {
                    commmand.CommandText = $"DROP TABLE {schema.Tables[i].Name};";
                    commmand.ExecuteNonQuery();
                }
            }
        }

        internal static void CreateSchema(IDbConnection connection, Schema schema, ITemplate template)
        {
            if (connection.State != ConnectionState.Open) connection.Open();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = template.Transform(schema);
                command.ExecuteNonQuery();
            }
        }
    }
}