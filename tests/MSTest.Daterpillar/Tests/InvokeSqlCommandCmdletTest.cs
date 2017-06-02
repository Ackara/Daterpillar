using Acklann.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.Data.SqlClient;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class InvokeSqlCommandCmdletTest
    {
        [TestMethod]
        public void Invoke_should_execute_mssql_command_when_a_connection_string_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmd1";
            var connection = ConnectionFactory.CreateMSSQLConnection(databaseName);
            var sut = new InvokeSqlCommandCmdlet()
            {
                Syntax = "mssql",
                ConnectionString = connection.ConnectionString,
                Script = $"create table foo (id int not null);"
            };

            // Act
            using (connection)
            {
                connection.UseEmptyDatabase();
            }

            var results = sut.Invoke<bool>().ToArray();
            var operationCompleted = results[0];

            // Assert
            results.Length.ShouldBe(1);
            operationCompleted.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_execute_mysql_command_when_a_connection_string_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmd1";
            var connection = ConnectionFactory.CreateMySQLConnection(databaseName);
            var builder = new SqlConnectionStringBuilder(connection.ConnectionString);
            var sut = new InvokeSqlCommandCmdlet()
            {
                Syntax = "mysql",
                User = builder.UserID,
                Host = builder.DataSource,
                Password = builder.Password,
                Database = builder.InitialCatalog,
                Script = $"CREATE TABLE foo (Id INT NOT NULL);"
            };

            // Act
            using (connection)
            {
                connection.UseEmptyDatabase();
            }

            var results = sut.Invoke<bool>().ToArray();
            var operationCompleted = results[0];

            // Assert
            results.Length.ShouldBe(1);
            operationCompleted.ShouldBeTrue();
        }

        
    }
}