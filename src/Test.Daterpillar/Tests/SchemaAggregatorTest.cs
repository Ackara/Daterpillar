using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Migration;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.IO;
using Tests.Daterpillar.Constants;
using Tests.Daterpillar.Helpers;

namespace Test.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(SampleData.Folder)]
    [DeploymentItem(KnownFile.DbConfig)]
    [DeploymentItem(KnownFile.x86SQLiteInterop)]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SchemaAggregatorTest
    {
        private static ScriptBuilderSettings _settings = new ScriptBuilderSettings()
        {
            AppendComments = false,
            AppendScripts = false,
            CreateDatabase = true,
            TruncateDatabaseIfItExist = false
        };

        [ClassCleanup]
        public static void Cleanup()
        {
            ApprovalTests.Maintenance.ApprovalMaintenance.CleanUpAbandonedFiles();
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void FetchSchema_should_return_a_schema_object_modeled_from_a_valid_db_connection()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ($"{nameof(Daterpillar)}-test.db3").ToLower());

            using (var connection = DatabaseHelper.CreateSQLiteConnection(fileName))
            {
                var builder = new SQLiteScriptBuilder(_settings);
                var sut = new SQLiteSchemaAggregator(connection);

                RunFetchSchemaTest(sut, builder, connection);
            }

#if !DEBUG
            if (File.Exists(fileName)) File.Delete(fileName);
#endif
        }

        public void RunFetchSchemaTest(ISchemaAggregator sut, IScriptBuilder builder, IDbConnection connection)
        {
            // Arrange
            var expectedSchema = Schema.Load(SampleData.GetFile(KnownFile.MockSchemaXML).OpenRead());

            // Act
            DatabaseHelper.TryDropDatabase(connection, connection.Database);
            DatabaseHelper.CreateSchema(connection, builder, expectedSchema);
            builder.Clear();
            var actualShema = sut.FetchSchema();

            builder.Create(expectedSchema);
            string expectedContent = builder.GetContent();

            builder.Clear();
            builder.Create(actualShema);
            string actualContent = builder.GetContent();

            // Assert
            Approvals.AssertEquals(expectedContent, actualContent);
        }
    }
}