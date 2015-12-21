using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem((Artifact.x86SQLiteInterop))]
    [DeploymentItem((Artifact.x64SQLiteInterop))]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.EmployeeSchema)]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class TemplateGenerationTests
    {
        /// <summary>
        /// Generate a SQLite schema from the <see cref="Artifact.EmployeeSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateSQLiteSchemaFromFile()
        {
            // Arrange
            var schema = Schema.Load(Samples.GetFile(Artifact.EmployeeSchema).OpenRead());
            var template = new SQLiteTemplate(new SQLiteTypeNameResolver(), addComments: true);

            // Act
            var sqlite = template.Transform(schema);
            var connection = DbFactory.CreateSQLiteConnection(sqlite);

            // Assert
            Assert.IsNotNull(connection);
            Assert.IsFalse(string.IsNullOrWhiteSpace(sqlite));
            Assert.IsTrue(File.Exists(connection.FileName));
        }

        /// <summary>
        /// Generate a Csharp classes from the <see cref="Artifact.EmployeeSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateCsharpClassesFromFile()
        {
            throw new System.NotImplementedException();
            // Arrange
            var schema = Schema.Load(Samples.GetFile(Artifact.EmployeeSchema).OpenRead());
            var template = new CsharpTemplate(CsharpTemplateSettings.Default, new CsharpTypeNameResolver());

            // Act
            var csharp = template.Transform(schema);

            // Assert
        }
    }
}