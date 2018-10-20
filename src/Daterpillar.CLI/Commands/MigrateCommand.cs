using Acklann.Daterpillar.Compilation;
using Acklann.Daterpillar.Configuration;
using Acklann.GlobN;
using Acklann.NShellit.Attributes;
using System;
using System.IO;
using System.Linq;

namespace Acklann.Daterpillar.Commands
{
    [Command("migrate", Cmdlet = "Update-DPSchema")]
    public class MigrateCommand : ICommand
    {
        [UseConstructor]
        public MigrateCommand(string oldSchema, string newSchema, string migrationsDirectory, string version, Syntax syntax = Syntax.Generic, string fileNameFormat = "V{0}__Update.sql", bool omitDropStatements = false)
        {
            Syntax = syntax;
            Version = version.Trim();
            FileNameFormat = fileNameFormat;
            OmitDropStatements = omitDropStatements;
            OldSchema = oldSchema.ResolvePath(Directory.GetCurrentDirectory()).FirstOrDefault() ?? oldSchema;
            NewSchema = newSchema.ResolvePath(Directory.GetCurrentDirectory()).FirstOrDefault() ?? newSchema;
            MigrationsDirectory = (string.IsNullOrEmpty(migrationsDirectory) ? Path.GetDirectoryName(oldSchema) : migrationsDirectory);
        }

        [Required, Parameter('f', "from", Kind = "path", Position = 0)]
        public string OldSchema { get; }

        [Required, Parameter('t', "to", Kind = "path", Position = 1)]
        public string NewSchema { get; }

        [Required, Parameter('v', "version", Position = 2)]
        public string Version { get; }

        [Parameter('o', "output", Kind = "path")]
        [Summary("The output/migrations directory. Defaults to <from> directory.")]
        public string MigrationsDirectory { get; }

        [Parameter('s', "syntax")]
        public Syntax Syntax { get; }

        [Parameter('d', "no-drops")]
        public bool OmitDropStatements { get; }

        [Parameter('f', "format")]
        public string FileNameFormat { get; }

        public int Execute()
        {
            if (File.Exists(NewSchema) == false) return Log.CouldNotFind(NewSchema, "schema");

            // Step 1: Merge (if any) referenced schema files into a new-schema.
            if (Schema.TryLoad(NewSchema, out Schema right, out string errorMsg))
                right.Merge();
            else
                return Log.NotWellFormedError(NewSchema, errorMsg);

            if (right.IsEmpty)
            {
                Log.PrintError($"The schema is empty at '{NewSchema}'.", ConsoleColor.Yellow);
                return 204;
            }

            // Step 2: Generate SQL migration script using the delta between the old and new schema.
            string dir;
            if (File.Exists(OldSchema) == false)
            {
                dir = Path.GetDirectoryName(OldSchema);
                if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);
                File.WriteAllText(OldSchema, new Schema().ToString());
            }

            if (Schema.TryLoad(OldSchema, out Schema left, out errorMsg) == false)
                return Log.NotWellFormedError(OldSchema, errorMsg);

            string scriptFile = Path.Combine(MigrationsDirectory, string.Format(FileNameFormat, Version, DateTime.Now, Syntax.ToString().ToLowerInvariant()).Trim());
            dir = Path.GetDirectoryName(scriptFile);
            if (Directory.Exists(dir) == false) Directory.CreateDirectory(dir);

            (new SqlMigrator()).GenerateMigrationScript(scriptFile, left, right, Syntax, OmitDropStatements);

            return 0;
        }
    }
}