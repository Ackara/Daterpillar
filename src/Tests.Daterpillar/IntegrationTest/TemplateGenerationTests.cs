using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(SampleData.MockSchemaXML)]
    [DeploymentItem((SampleData.x86SQLiteInterop))]
    [DeploymentItem((SampleData.x64SQLiteInterop))]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class TemplateGenerationTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Transform_should_generate_a_sqlite_schema_when_invoked()
        {
            // Arrange
            var settings = new SQLiteTemplateSettings()
            {
                CommentsEnabled = true,
                DropTableIfExist = true
            };

            var schema = Schema.Load(SampleData.GetFile(SampleData.MockSchemaXML).OpenRead());
            var sut = new SQLiteTemplate(settings, new SQLiteTypeNameResolver());

            // Act
            var script = sut.Transform(schema);
            var connection = DbFactory.CreateSQLiteConnection(script);

            // Assert
            Assert.IsNotNull(connection);
            Assert.IsFalse(string.IsNullOrWhiteSpace(script));
            Assert.IsTrue(File.Exists(connection.FileName));
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Transform_should_generate_a_csharp_classes_when_invoked()
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(SampleData.MockSchemaXML).OpenRead());
            var sut = new CSharpTemplate(CSharpTemplateSettings.Default, new CSharpTypeNameResolver());

            // Act
            var csharp = sut.Transform(schema);

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
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Transform_should_generate_csharp_classes_that_implement_INotifyPropertyChanged_when_invoked()
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(SampleData.MockSchemaXML).OpenRead());
            var settings = new NotifyPropertyChangedTemplateSettings()
            {
                DataContractsEnabled = true,
                SchemaAnnotationsEnabled = true,
                VirtualPropertiesEnabled = true,
                PartialRaisePropertyChangedMethodEnabled = true,
            };
            var sut = new NotifyPropertyChangedTemplate(settings, new CSharpTypeNameResolver());

            // Act
            var csharp = sut.Transform(schema);

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
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Transform_should_generate_a_mysql_schema_when_invoked()
        {
            // Arrange
            using (var fileStream = SampleData.GetFile(SampleData.MockSchemaXML).OpenRead())
            {
                var settings = new MySqlTemplateSettings()
                {
                    CommentsEnabled = true,
                    DropDatabaseIfExist = true
                };

                var schema = Schema.Load(fileStream);
                var sut = new MySqlTemplate(settings, new MySqlTypeNameResolver());
                int nChanges;

                // Act
                var script = sut.Transform(schema);
                TestContext.WriteLine(script);

                using (var connection = DbFactory.CreateMySqlConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = script;
                        nChanges = command.ExecuteNonQuery();
                    }
                }

                // Assert
                Assert.IsTrue(nChanges > 0, $"{nChanges} changes were made.");
                Assert.IsFalse(string.IsNullOrWhiteSpace(script));
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Transform_should_generate_a_tsql_schema_when_invoked()
        {
            // Arrange
            var settings = new SqlTemplateSettings()
            {
                AddScript = true,
                CreateSchema = false,
                UseDatabase = true,
                CommentsEnabled = true,
                DropDatabaseIfExist = false,
            };

            var schema = Schema.Load(SampleData.GetFile(SampleData.MockSchemaXML).OpenRead());
            var script = new SqlTemplate(settings).Transform(schema);

            // Act
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = script;
                    command.ExecuteNonQuery();
                }

                // Cleanup
                SampleData.TruncateDatabase(connection, schema);
            }

            // Assert
            Approvals.Verify(script);
        }
    }
}