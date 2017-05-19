using Ackara.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Data;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class ImportSQLDataCmdletTest
    {
        [TestMethod]
        public void Invoke_should_import_table_records_from_one_database_to_the_next()
        {
            // Arrange
            int recordCount;
            var source = ConnectionFactory.CreateMySQLConnection("dtpl_import1");
            var target = ConnectionFactory.CreateMySQLConnection("dtpl_import2");

            var sut = new ImportSQLDataCmdlet()
            {
                Syntax = "mysql",
                Table = "ability",
                Source = source.ConnectionString,
                Destination = target.ConnectionString
            };

            // Act
            using (source)
            {
                source.UseSchema(FName.scriptingTest_partial_schemaXML);

                using (target)
                {
                    target.UseSchema(FName.scriptingTest_partial_schemaXML);
                    var truncateFailed = !target.TryExecuteScript($"DELETE FROM {sut.Table};", out string errorMsg);
                    if (truncateFailed) Assert.Fail($"Failed to truncate the table. " + errorMsg);
                }
            }

            var results = sut.Invoke<DataTable>().ToArray();
            using (target = ConnectionFactory.CreateMySQLConnection("dtpl_import2"))
            {
                target.Open();
                using (var cmd = target.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM {sut.Table};";
                    using (var data = new DataTable())
                    {
                        data.Load(cmd.ExecuteReader());
                        recordCount = data.Rows.Count;
                    }
                }
            }

            // Assert
            results.Length.ShouldBe(1);
            recordCount.ShouldBeGreaterThanOrEqualTo(3);
        }

        [TestMethod]
        public void Invoke_should_import_table_records_from_one_server_to_the_next()
        {
            // Arrange
            int recordCount;
            var source = ConnectionFactory.CreateMySQLConnection("dtpl_import1");
            var target = ConnectionFactory.CreateSQLiteConnection("dtpl_import2");

            var sut = new ImportSQLDataCmdlet()
            {
                Syntax = "mysql",
                Table = "ability",
                Source = new MySql.Data.MySqlClient.MySqlConnection(source.ConnectionString),
                Destination = new System.Data.SQLite.SQLiteConnection(target.ConnectionString)
            };

            // Act
            using (source)
            {
                source.UseSchema(FName.scriptingTest_partial_schemaXML);

                using (target)
                {
                    target.UseSchema(FName.scriptingTest_partial_schemaXML);
                    var truncateFailed = !target.TryExecuteScript($"DELETE FROM {sut.Table};", out string errorMsg);
                    if (truncateFailed) Assert.Fail($"Failed to truncate the table. " + errorMsg);
                }
            }

            var results = sut.Invoke<DataTable>().ToArray();
            using (target = ConnectionFactory.CreateSQLiteConnection("dtpl_import2"))
            {
                target.Open();
                using (var cmd = target.CreateCommand())
                {
                    cmd.CommandText = $"SELECT * FROM {sut.Table};";
                    using (var data = new DataTable())
                    {
                        data.Load(cmd.ExecuteReader());
                        recordCount = data.Rows.Count;
                    }
                }
            }

            // Assert
            results.Length.ShouldBe(1);
            recordCount.ShouldBeGreaterThanOrEqualTo(3);
        }
    }
}