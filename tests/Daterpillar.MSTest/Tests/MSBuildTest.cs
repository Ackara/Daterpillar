using ApprovalTests;
using FakeItEasy;
using Microsoft.Build.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;

namespace Acklann.Daterpillar.Tests
{
    [TestClass]
    public class MSBuildTest
    {
        //[TestMethod]
        public void Can_generate_schema_file_from_project_file()
        {
            // Arrange

            var projectPath = Environment.GetEnvironmentVariable("THIS_PROJECT_FILE");
            var resultFile = Path.Combine(Path.GetTempPath(), $"msbuild-schema-export-test.xml");

            var host = A.Fake<ITaskHost>();
            var engine = A.Fake<IBuildEngine>();
            var projectFile = A.Fake<ITaskItem>();

            A.CallTo(() => projectFile.GetMetadata("FullPath")).Returns(projectPath);

            var sut = new Targets.ExportDatabaseSchema
            {
                ProjectFile = projectFile,
                OutputFilePath = resultFile,
                EntryType = typeof(Prototyping.Song).FullName,
                BuildEngine = engine,
                HostObject = host
            };

            // Act

            if (File.Exists(resultFile)) File.Delete(resultFile);
            var pass = sut.Execute();

            // Assert

            pass.ShouldBeTrue();
            File.Exists(resultFile).ShouldBeTrue();
            Approvals.VerifyFile(resultFile);
        }
    }
}