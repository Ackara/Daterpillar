using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.SampleSchema)]
    [DeploymentItem(Artifact.TSqlSampleSchema)]
    [DeploymentItem((Artifact.x86SQLiteInterop))]
    [DeploymentItem((Artifact.x64SQLiteInterop))]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(FileLauncherReporter), typeof(ClipboardReporter))]
    public class TemplateGenerationTests
    {
        public TestContext TestContext { get; set; }

        [ClassCleanup]
        public static void Cleanup()
        {
            string connStr = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
            var conn = new System.Data.SqlClient.SqlConnection(connStr);

            using (conn)
            {
                conn.Open();
                using (var command = conn.CreateCommand())
                {
                    command.CommandText = "CREATE DATABASE [zune];";
                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Generate a SQLite schema from the <see cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateSQLiteSchemaFromFile()
        {
            // Arrange
            var settings = new SQLiteTemplateSettings()
            {
                CommentsEnabled = true,
                DropTableIfExist = true
            };

            var schema = Schema.Load(SampleData.GetFile(Artifact.SampleSchema).OpenRead());
            var template = new SQLiteTemplate(settings, new SQLiteTypeNameResolver());

            // Act
            var script = template.Transform(schema);
            var connection = DbFactory.CreateSQLiteConnection(script);

            // Assert
            Assert.IsNotNull(connection);
            Assert.IsFalse(string.IsNullOrWhiteSpace(script));
            Assert.IsTrue(File.Exists(connection.FileName));
        }

        /// <summary>
        /// Generate a CSharp classes from the <see cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateCSharpClassesFromFile()
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(Artifact.SampleSchema).OpenRead());
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

        /// <summary>
        /// Generate a CSharp classes that implement <see
        /// cref="System.ComponentModel.INotifyPropertyChanged"/> from the <see
        /// cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateNotifyPropertyChangedClassesFromFile()
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(Artifact.SampleSchema).OpenRead());
            var template = new NotifyPropertyChangedTemplate(NotifyPropertyChangedTemplateSettings.Default, new CSharpTypeNameResolver());

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

        /// <summary>
        /// Generate a CSharp classes that implement <see
        /// cref="System.ComponentModel.INotifyPropertyChanged"/> with a partial method from the
        /// <see cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void GenerateNotifyPropertyChangedClassesWithPartialMethodFromFile()
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(Artifact.SampleSchema).OpenRead());
            var settings = new NotifyPropertyChangedTemplateSettings() { PartialRaisePropertyChangedMethodEnabled = true };
            var template = new NotifyPropertyChangedTemplate(settings, new CSharpTypeNameResolver());

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

        /// <summary>
        /// Generate a MySQL schema from the <see cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        //[Ignore(/* To run this test provide a connection string to a MySQL database in the app.config. */)]
        public void GenerateMySqlSchemaFromFile()
        {
            // Arrange
            using (var fileStream = SampleData.GetFile(Artifact.SampleSchema).OpenRead())
            {
                var settings = new MySqlTemplateSettings()
                {
                    CommentsEnabled = true,
                    DropDatabaseIfExist = true
                };

                var schema = Schema.Load(fileStream);
                var template = new MySqlTemplate(settings, new MySqlTypeNameResolver());
                int nChanges;

                // Act
                var mysql = template.Transform(schema);
                TestContext.WriteLine(mysql);

                using (var connection = DbFactory.CreateMySqlConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = mysql;
                        nChanges = command.ExecuteNonQuery();
                    }
                }

                // Assert
                Assert.IsTrue(nChanges > 0, $"{nChanges} changes were made.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(mysql));
            }
        }

        /// <summary>
        /// Create a SQL Server schema from the <see cref="Artifact.SampleSchema"/> file.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        //[Ignore(/* To run this test provide a connection string to a SQL Server database in the app.config. */)]
        public void GenerateSqlSchemaFromFile()
        {
            // Arrange
            var settings = new SqlTemplateSettings()
            {
                AddScript = false,
                CommentsEnabled = true,
                DropDatabaseIfExist = true
            };

            var schema = Schema.Load(SampleData.GetFile(Artifact.TSqlSampleSchema).OpenRead());
            var script = new SqlTemplate(settings).Transform(schema);

            // Act
            using (var connection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}