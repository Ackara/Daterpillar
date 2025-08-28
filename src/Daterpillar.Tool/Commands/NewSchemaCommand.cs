using Acklann.Daterpillar.Modeling;
using CommandLine;
using System.IO;

namespace Acklann.Daterpillar.Tool.Commands
{
    [Verb("schema", HelpText = "Creates a new schema .xml file.")]
    public class NewSchemaCommand : ICommand
    {
        [Option('p', "path", Required = true, HelpText = "The absolute path to where the file will be saved.")]
        public string SchemaFilePath { get; set; }

        [Option('n', "name", HelpText = "The name of the schema.")]
        public string SchemaName { get; set; }

        public int Execute()
        {
            string dir = Path.GetDirectoryName(SchemaFilePath);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            var schema = new Schema(SchemaName);
            using Stream file = new FileStream(SchemaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read);
            schema.WriteTo(file);

            return 0;
        }
    }
}