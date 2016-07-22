using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using System.Data;

namespace Tests.Daterpillar
{
    public static partial class SampleData
    {
        #region Database Methods

        public static bool TryCreateSampleDatabase(IDbConnection connection, ITemplate template)
        {
            var schema = Schema.Load(Test.Data.GetFile(Test.File.MockSchemaXML).OpenRead());
            return TryCreateSampleDatabase(connection, schema, template);
        }

        public static bool TryCreateSampleDatabase(IDbConnection connection, Schema schema, ITemplate template)
        {
            try
            {
                using (connection)
                {
                    if (connection.State != ConnectionState.Open) connection.Open();
                    using (IDbCommand command = connection.CreateCommand())
                    {
                        command.CommandText = template.Transform(schema);
                        command.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (System.Data.Common.DbException ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.ErrorCode}");
                System.Diagnostics.Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public static void TruncateDatabase(IDbConnection connection, Schema schema)
        {
            using (connection)
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                using (var command = connection.CreateCommand())
                {
                    for (int i = schema.Tables.Count - 1; i >= 0; i--)
                    {
                        command.CommandText = $"DROP TABLE {schema.Tables[i].Name};";
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #endregion Database Methods
    }
}