using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Acklann.Daterpillar;
using Acklann.Daterpillar.Migration;
using Acklann.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data;
using System.IO;
using System.Text;
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
        public void FetchSchema_should_return_a_schema_object_modeled_from_a_valid_tsql_database()
        {
            using (var connection = DatabaseHelper.CreateMSSQLConnection())
            {
                var builder = new MSSQLScriptBuilder(_settings);
                var sut = new MSSQLSchemaAggregator(connection);

                RunFetchSchemaTest(sut, builder, connection);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Trait.Integration)]
        public void FetchSchema_should_return_a_schema_object_modeled_from_a_valid_mysql_database()
        {
            using (var connection = DatabaseHelper.CreateMySQLConnection())
            {
                var builder = new MySQLScriptBuilder(_settings);
                var sut = new MySQLSchemaAggregator(connection);

                RunFetchSchemaTest(sut, builder, connection);
            }
        }

        public void RunFetchSchemaTest(ISchemaAggregator sut, IScriptBuilder builder, IDbConnection connection)
        {
            // Arrange
            var schema = Schema.Load(SampleData.GetFile(KnownFile.MockSchema2XML).OpenRead());

            Func<Schema, string> getContent = (x) =>
            {
                //builder.Clear();
                //builder.Create(x);
                //return builder.GetContent();
                using (var stream = new MemoryStream())
                {
                    x.WriteTo(stream);
                    return Encoding.UTF8.GetString(stream.ToArray());
                }
            };

            // Act
            DatabaseHelper.TryDropDatabase(connection, schema.Name);
            DatabaseHelper.CreateSchema(connection, builder, schema);
            connection.ChangeDatabase(schema.Name);

            string expectedResult = getContent(schema);
            string actualResult = getContent(sut.FetchSchema());

            // Assert
            Approvals.AssertEquals(expectedResult, actualResult);
        }
    }
}