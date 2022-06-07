using Acklann.Daterpillar.Modeling;
using Acklann.Daterpillar.Tool.Commands;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class CommandToolTest
    {
        [TestMethod]
        public void Can_generate_migration_script_from_two_schemas()
        {
            // Arrange

            var schema1 = SchemaFactory.CreateFrom(typeof(Prototyping.Song).Assembly);
            var schema2 = SchemaFactory.CreateFrom(typeof(Prototyping.Song).Assembly);
            schema2.Tables[0].Name = "Foobar";

            var scriptPath = Path.Combine(Path.GetTempPath(), "generated-tool-migration.sql");
            var schema1Path = Path.Combine(Path.GetTempPath(), "old-test-schema.xml");
            var schema2Path = Path.Combine(Path.GetTempPath(), "new-test-schema.xml");
            schema1.Save(schema1Path);
            schema2.Save(schema2Path);

            var sut = new GenerateCommand
            {
                OldSchemaPath = schema1Path,
                NewSchemaPath = schema2Path,
                OutputFile = scriptPath,
                Language = Language.MySQL
            };

            // Act + Assert

            sut.Execute().ShouldBe(0);
            File.Exists(scriptPath).ShouldBeTrue();
            File.ReadAllText(scriptPath).ShouldNotBeNullOrWhiteSpace();
        }

        //[TestMethod]
        public void Can_generate_schema_file_from_project_file()
        {
            // Arrange

            var projectPath = Environment.GetEnvironmentVariable("THIS_PROJECT_FILE");
            var resultFile = Path.Combine(Path.GetTempPath(), $"msbuild-schema-export-test.xml");

            var sut = new ExportCommand
            {
                ProjectFile = projectPath,
                OutputFilePath = resultFile,
                EntryType = typeof(Prototyping.Song).FullName
            };

            // Act

            if (File.Exists(resultFile)) File.Delete(resultFile);
            var pass = sut.Execute();

            // Assert

            pass.ShouldBe(0);
            File.Exists(resultFile).ShouldBeTrue();
            Approvals.VerifyFile(resultFile);
        }
    }
}