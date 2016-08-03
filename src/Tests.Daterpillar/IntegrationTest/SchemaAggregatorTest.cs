using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Data;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    public class SchemaAggregatorTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_mssql_database()
        {
            // Arrange
            var schema = Test.Data.CreateMockSchema();
            var connection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString);
            IgnoreTestIfDbConnectionIsUnavailable(schema, connection, new SqlTemplate());

            // Act

            // Assert
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_mysql_database()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        [TestCategory(Test.Trait.Integration)]
        public void FetchSchema_should_build_a_schema_object_from_a_active_sqlite_database()
        {
            throw new System.NotImplementedException();
        }

        #region Private Members

        private static void IgnoreTestIfDbConnectionIsUnavailable(Schema schema, IDbConnection connection, ITemplate template)
        {
            bool wasNotSuccessful = !Test.Data.TryCreateDatabase(connection, schema, template);
#if DEBUG
            if (wasNotSuccessful) Assert.Inconclusive();
#else
            throw new System.ArgumentException($"Cannot connection to '{connection.Database}' database.");
#endif
        }

        #endregion Private Members
    }
}