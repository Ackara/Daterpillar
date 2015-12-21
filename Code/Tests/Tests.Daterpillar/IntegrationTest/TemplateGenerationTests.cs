using Ackara.Daterpillar.Transformation;
using Ackara.Daterpillar.Transformation.Template;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [UseApprovalSubdirectory(Str.ApprovalsDir)]
    [DeploymentItem((Artifact.x86SQLiteInterop))]
    [DeploymentItem((Artifact.x64SQLiteInterop))]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.EmployeeSchema)]
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
        /// Generate a CSharp classes from the <see cref="Artifact.EmployeeSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateCSharpClassesFromFile()
        {
            // Arrange
            var schema = Schema.Load(Samples.GetFile(Artifact.EmployeeSchema).OpenRead());
            var template = new CSharpTemplate(CSharpTemplateSettings.Default, new CSharpTypeNameResolver());

            // Act
            var csharp = template.Transform(schema);

            var syntax = CSharpSyntaxTree.ParseText(csharp);
            var errorList = syntax.GetDiagnostics();
            int errorCount = 0;

            foreach (var error in syntax.GetDiagnostics())
            {
                errorCount++;
                Debug.WriteLine(error);
            }

            // Assert
            Assert.AreEqual(0, errorCount);
            Assert.IsFalse(string.IsNullOrWhiteSpace(csharp));
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void GenerateMySqlSchemaFromFile()
        {
            // Arrange
            using (var fileStream = Samples.GetFile(Artifact.EmployeeSchema).OpenRead())
            {
                var schema = Schema.Load(fileStream);
                var template = new MySqlTemplate(new MySqlTypeNameResolver(), addComment: true);
                int exitcode;

                // Act
                var mysql = template.Transform(schema);

                using (var connection = DbFactory.CreateMySqlConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = mysql;
                        exitcode = command.ExecuteNonQuery();
                    }
                }

                // Assert
                Assert.AreEqual(0, exitcode);
                Assert.IsFalse(string.IsNullOrWhiteSpace(mysql));
            }
        }
    }
}