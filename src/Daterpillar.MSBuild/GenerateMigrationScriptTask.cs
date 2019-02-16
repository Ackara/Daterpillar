using Acklann.Daterpillar.Compilation;
using Acklann.Daterpillar.Configuration;
using Microsoft.Build.Framework;
using System;
using System.IO;

namespace Acklann.Daterpillar
{
    public class GenerateMigrationScriptTask : ITask
    {
        [Required]
        public string OldSchemaFilePath { get; set; }

        [Required]
        public string NewSchemaFilePath { get; set; }

        [Required]
        public string MigrationsDirectory { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string Language { get; set; }

        public bool Execute()
        {
            bool successful = false;

            if (File.Exists(OldSchemaFilePath) == false)
            {
                Helper.CreateDirectory(OldSchemaFilePath);
                new SchemaDeclaration().Save(OldSchemaFilePath);
            }

            if (SchemaDeclaration.TryLoad(OldSchemaFilePath, out SchemaDeclaration oldSchema, out string errorMsg) == false)
                BuildEngine.Error($"{nameof(Daterpillar)} | {errorMsg}");

            if (SchemaDeclaration.TryLoad(NewSchemaFilePath, out SchemaDeclaration newSchema, out errorMsg) == false)
                BuildEngine.Error($"{nameof(Daterpillar)} | {errorMsg}");

            if (oldSchema != null && newSchema != null)
                if (Conversion.EnumConverter.TryConvertToLanguage(Language, out Syntax lang))
                {
                    string scriptFilePath = Path.ChangeExtension(Path.Combine(MigrationsDirectory, FileName), $"{lang.ToString().ToLowerInvariant()}.sql");
                    var factory = new SqlWriterFactory();
                    var migrator = new SqlMigrator();

                    Helper.CreateDirectory(scriptFilePath);
                    using (var stream = new FileStream(scriptFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (var writer = new StreamWriter(stream))
                    {
                        migrator.GenerateMigrationScript(factory.CreateInstance(lang, writer), oldSchema, newSchema);
                        BuildEngine.Info(MessageImportance.High, $"{nameof(Daterpillar)} => created '{scriptFilePath}'");
                        successful = true;
                    }
                }
                else BuildEngine.Error($"{nameof(Daterpillar)} | The 'Language' parameter must be one of the following values ({string.Join(", ", Enum.GetNames(typeof(Syntax)))})");

            return successful;
        }

        #region ITask

        public IBuildEngine BuildEngine { get; set; }
        public ITaskHost HostObject { get; set; }

        #endregion ITask
    }
}