using Gigobyte.Daterpillar.Arguments;
using Gigobyte.Daterpillar.TextTransformation;
using System.Data;
using System.IO;

namespace Gigobyte.Daterpillar.Commands
{
    [VerbLink(MountVerb.Name)]
    public class MountCommand : CommandBase
    {
        public override int Execute(object args)
        {
            var options = (MountVerb)args;

            var schema = Schema.Load(File.OpenRead(options.Path));
            ITemplate template = new TemplateFactory().CreateInstance(options.Platform);
            IDbConnection connection = GetConnection(options.Platform, options.ConnectionString);

            if (options.Override) TruncateDatabase(schema, connection);
            ExecuteSchema(schema, connection, template);
            return ExitCode.Success;
        }

        private void TruncateDatabase(Schema schema, IDbConnection connection)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                for (int i = schema.Tables.Count - 1; i >= 0; i--)
                {
                    try { command.CommandText = $"DROP TABLE {schema.Tables[i].Name};"; }
                    catch (System.Data.Common.DbException) { }
                }
            }
        }

        private void ExecuteSchema(Schema schema, IDbConnection connection, ITemplate template)
        {
            if (connection.State != ConnectionState.Open) connection.Open();

            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText = template.Transform(schema);
                command.ExecuteNonQuery();
            }
        }
    }
}