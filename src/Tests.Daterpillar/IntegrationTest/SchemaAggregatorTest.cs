using ApprovalTests;
using ApprovalTests.Namers;
using ApprovalTests.Reporters;
using Gigobyte.Daterpillar;
using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.IO;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [UseApprovalSubdirectory(nameof(ApprovalTests))]
    [UseReporter(typeof(DiffReporter), typeof(ClipboardReporter))]
    public class SchemaAggregatorTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_mssql_database()
        {
            // Arrange
            var connectionString = Test.ConnectionString.MSSQL;
            IgnoreTestIfDbConnectionIsUnavailable(SampleData.CreateMockSchema(), new System.Data.SqlClient.SqlConnection(connectionString), new MSSQLTemplate());

            var sut = new MSSQLSchemaAggregator(new System.Data.SqlClient.SqlConnection(connectionString));

            // Act
            var remoteSchema = sut.FetchSchema();
            var memoryStream = new MemoryStream();
            remoteSchema.WriteTo(memoryStream);

            var validator = new Utilities.XmlValidator();
            validator.Load(memoryStream);

            // Assert
            Approvals.VerifyBinaryFile(memoryStream.ToArray(), "xml");
            Assert.IsTrue(validator.XmlDocIsValid);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_mysql_database()
        {
            // Arrange
            var connectionString = Test.ConnectionString.MySQL;
            IgnoreTestIfDbConnectionIsUnavailable(SampleData.CreateMockSchema(), new MySql.Data.MySqlClient.MySqlConnection(connectionString), new MySQLTemplate());

            var sut = new MySQLSchemaAggregator(new MySql.Data.MySqlClient.MySqlConnection(connectionString));

            // Act
            var remoteSchema = sut.FetchSchema();
            var memoryStream = new MemoryStream();
            remoteSchema.WriteTo(memoryStream);

            var validator = new Utilities.XmlValidator();
            validator.Load(memoryStream);

            // Assert
            Approvals.VerifyBinaryFile(memoryStream.ToArray(), "xml");
            Assert.IsTrue(validator.XmlDocIsValid);
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_sqlite_database()
        {
            // Arrange
            var schema = SampleData.CreateMockSchema();
            var connStr = new System.Data.SQLite.SQLiteConnectionStringBuilder(Test.ConnectionString.SQLite);
            if (!File.Exists(connStr.DataSource)) System.Data.SQLite.SQLiteConnection.CreateFile(connStr.DataSource);
            IgnoreTestIfDbConnectionIsUnavailable(schema, new System.Data.SQLite.SQLiteConnection(connStr.ConnectionString), new SQLiteTemplate());

            var sut = new SQLiteSchemaAggregator(new System.Data.SQLite.SQLiteConnection(connStr.ConnectionString));

            // Act
            var remoteSchema = sut.FetchSchema();
            var memoryStream = new MemoryStream();
            remoteSchema.WriteTo(memoryStream);

            var validator = new Utilities.XmlValidator();
            validator.Load(memoryStream);

            // Assert
            Approvals.VerifyBinaryFile(memoryStream.ToArray(), "xml");
            Assert.IsTrue(validator.XmlDocIsValid);
        }

        #region Private Members

        private static void IgnoreTestIfDbConnectionIsUnavailable(Schema schema, IDbConnection connection, ITemplate template)
        {
            bool wasNotSuccessful = !SampleData.TryCreateDatabase(connection, schema, template);
#if DEBUG
            if (wasNotSuccessful) Assert.Inconclusive();
#else
            if (wasNotSuccessful) throw new System.ArgumentException($"Cannot connection to '{connection.Database}' database.");
#endif
        }

        #endregion Private Members
    }
}