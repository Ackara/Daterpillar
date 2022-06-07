using Acklann.Daterpillar.Modeling;
using CommandLine;
using System;
using System.IO;
using System.Xml.Schema;

namespace Acklann.Daterpillar.Commands
{
    [Verb("generate", HelpText = "Produces a migration script from the old schema to the new schema.")]
    public class GenerateCommand : ICommand
    {
        [Option('f', "old-schema", Required = true)]
        public string OldSchemaPath { get; set; }

        [Option('t', "new-schema", Required = true)]
        public string NewSchemaPath { get; set; }

        [Option('o', "output", Required = true)]
        public string OutputFile { get; set; }

        [Option('l', "language", Required = true)]
        public Acklann.Daterpillar.Language Language { get; set; }

        public int Execute()
        {
            if (!File.Exists(OldSchemaPath)) throw new FileNotFoundException($"Could not find file at '{OldSchemaPath}'.");
            if (!File.Exists(NewSchemaPath)) throw new FileNotFoundException($"Could not find file at '{NewSchemaPath}'.");
            if (string.IsNullOrWhiteSpace(OutputFile)) throw new ArgumentNullException(nameof(OutputFile));
            if (!Schema.TryLoad(OldSchemaPath, out Schema oldSchema, PrintErrors)) throw new InvalidDataException();
            if (!Schema.TryLoad(NewSchemaPath, out Schema newSchema, PrintErrors)) throw new InvalidDataException();

            new Migrator().GenerateMigrationScript(Language, oldSchema, newSchema, OutputFile);
            return 0;
        }

        private void PrintErrors(XmlSeverityType level, XmlSchemaException error)
        {
            Console.Error.WriteLine($"[{level}] {error.Message}");
        }
    }
}