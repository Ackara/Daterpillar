using Acklann.Daterpillar.Modeling;
using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace Acklann.Daterpillar.Tool.Commands
{
    [Verb("generate", HelpText = "Produces a migration script from the old schema to the new schema.")]
    public class GenerateCommand : ICommand
    {
        [Option('f', "old-schema", Required = true, HelpText = "The path of the old schema file.")]
        public string OldSchemaPath { get; set; }

        [Option('t', "new-schema", Required = true, HelpText = "The path of the new schema file.")]
        public string NewSchemaPath { get; set; }

        [Option('o', "output", Required = true, HelpText = "The path of the generated migration script.")]
        public string OutputFile { get; set; }

        [Option('l', "language", Required = true, HelpText = "The SQL language")]
        public Acklann.Daterpillar.Language Language { get; set; }

        public int Execute()
        {
            if (string.IsNullOrWhiteSpace(NewSchemaPath)) throw new ArgumentNullException(nameof(NewSchemaPath));
            if (string.IsNullOrWhiteSpace(OldSchemaPath)) throw new ArgumentNullException(nameof(OldSchemaPath));
            if (string.IsNullOrWhiteSpace(OutputFile)) throw new ArgumentNullException(nameof(OutputFile));
            if (!Schema.TryLoad(OldSchemaPath, out Schema oldSchema, PrintErrors)) oldSchema = new Schema();
            if (!Schema.TryLoad(NewSchemaPath, out Schema newSchema, PrintErrors)) newSchema = new Schema();

            new Migrator().GenerateMigrationScript(Language, oldSchema, newSchema, OutputFile);
            return 0;
        }

        private void PrintErrors(XmlSeverityType level, XmlSchemaException error)
        {
            Console.Error.WriteLine($"[{level}] {error.Message}");
        }

        [Usage(ApplicationAlias = "daterpillar")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                yield return new Example("create migration script", new GenerateCommand
                {
                    OldSchemaPath = @"C:\old-schema.xml",
                    NewSchemaPath = @"C:\new-schema.xml",
                    OutputFile = @"C:\V1.0__init.sql",
                    Language = Language.MySQL
                });
            }
        }
    }
}