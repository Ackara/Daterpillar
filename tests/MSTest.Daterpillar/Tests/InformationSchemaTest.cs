using Acklann.Daterpillar.Migration;
using Acklann.Daterpillar.Scripting;
using ApprovalTests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.X86)]  
    [DeploymentItem(FName.Samples)]
    public class InformationSchemaTest
    {
        [TestMethod]
        public void FetchSchema_should_return_schema_object_based_on_an_existing_mysql_database()
        {
            using (var connection = ConnectionFactory.CreateMySQLConnection(DBNAME))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
                var sut = new MySQLInformationSchema(connection);
                var result = sut.FetchSchema(DBNAME);
                var settings = new SqlScriptBuilderSettings()
                {
                    AppendCreateSchemaCommand = false,
                    IgnoreComments = true,
                    IgnoreScripts = true,
                    ShowHeader = false,
                };

                var mysql = new MySQLScriptBuilder(settings)
                    .Append(result)
                    .GetContent();

                Approvals.Verify(mysql);
            }
        }

        [TestMethod]
        public void FetchSchema_should_return_schema_object_based_on_an_existing_mssql_database()
        {
            using (var connection = ConnectionFactory.CreateMSSQLConnection(DBNAME))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
                var sut = new MSSQLInformationSchema(connection);
                var result = sut.FetchSchema(DBNAME);
                var settings = new SqlScriptBuilderSettings()
                {
                    AppendCreateSchemaCommand = false,
                    IgnoreComments = true,
                    IgnoreScripts = true,
                    ShowHeader = false,
                };

                var mssql = new MSSQLScriptBuilder(settings)
                    .Append(result)
                    .GetContent();

                Approvals.Verify(mssql);
            }
        }

        [TestMethod]
        public void FetchSchema_should_return_schema_object_based_on_an_existing_sqlite_database()
        {
            using (var connection = ConnectionFactory.CreateSQLiteConnection(DBNAME))
            {
                connection.UseSchema(FName.scriptingTest_partial_schemaXML);
                var sut = new SQLiteInformationSchema(connection);
                var result = sut.FetchSchema(DBNAME);
                var settings = new SqlScriptBuilderSettings()
                {
                    AppendCreateSchemaCommand = false,
                    IgnoreComments = true,
                    IgnoreScripts = true,
                    ShowHeader = false,
                };

                var mssql = new SQLiteScriptBuilder(settings)
                    .Append(result)
                    .GetContent();

                Approvals.Verify(mssql);
            }
        }

        private const string DBNAME = "dtpl_info_mockDb";
    }
}