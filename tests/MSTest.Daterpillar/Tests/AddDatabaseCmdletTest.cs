using Acklann.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.Data;
using System.IO;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class AddDatabaseCmdletTest
    {
        [TestMethod]
        public void Invoke_should_add_a_database_to_the_specified_server_when_a_connection_string_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmdlet_add";
            var connectionString = ConnectionFactory.GetMySQLConnectionString();
            var sut = new AddDatabaseCmdlet()
            {
                ConnectionString = connectionString,
                Database = databaseName,
                Syntax = "mysql"
            };

            // Act
            using (var connection = ConnectionFactory.CreateMySQLConnection())
            {
                bool commandFailed = !connection.TryExecuteScript($"DROP DATABASE IF EXISTS `{databaseName}`;", out string errorMsg);
                if (commandFailed) Assert.Fail(errorMsg);
            }

            var results = sut.Invoke<bool>().ToArray();
            var databaseCreated = results[0];
            using (var connection = ConnectionFactory.CreateMySQLConnection(databaseName))
            {
                connection.Open();
            }

            // Assert
            results.Length.ShouldBe(1);
            databaseCreated.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_add_a_database_to_the_specified_server_when_a_all_args_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmdlet_add3";
            var connectionString = ConnectionFactory.GetMySQLConnectionString(databaseName);
            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString);
            var sut = new AddDatabaseCmdlet()
            {
                Host = builder.Server,
                User = builder.UserID,
                Password = builder.Password,
                Database = builder.Database,
                Syntax = "mysql"
            };

            // Act
            using (var connection = ConnectionFactory.CreateMySQLConnection())
            {
                bool commandFailed = !connection.TryExecuteScript($"DROP DATABASE IF EXISTS `{databaseName}`;", out string errorMsg);
                if (commandFailed) Assert.Fail(errorMsg);
            }

            var results = sut.Invoke<bool>().ToArray();
            var databaseCreated = results[0];
            using (var connection = ConnectionFactory.CreateMySQLConnection(databaseName))
            {
                connection.Open();
            }

            // Assert
            results.Length.ShouldBe(1);
            databaseCreated.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_create_a_sqlite_database_when_a_all_args_is_passed()
        {
            // Arrange
            var databaseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameof(AddDatabaseCmdletTest)}.db3");
            var sut = new AddDatabaseCmdlet()
            {
                Host = databaseFilePath,
                Syntax = "sqlite"
            };

            // Act
            if (File.Exists(databaseFilePath)) File.Delete(databaseFilePath);
            var results = sut.Invoke<bool>().ToArray();
            var operationCompleted = results[0];
            var dbFileExist = File.Exists(databaseFilePath);

            // Assert
            results.Length.ShouldBe(1);
            dbFileExist.ShouldBeTrue();
            operationCompleted.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_recreate_a_mysql_database_that_already_exist()
        {
            // Arrange
            int totalTables;
            bool databaseWasCreated;
            string dbName = "dtpl_dltIfExist";
            var connection = ConnectionFactory.CreateMySQLConnection(dbName);
            var sut = new AddDatabaseCmdlet()
            {
                Database = dbName,
                Syntax = "mysql",
                DeleteIfExist = true,
                ConnectionString = connection.ConnectionString,
            };

            using (connection)
            {
                connection.UseSchema(FName.cmdlets_source_schemaXML);
                var results = sut.Invoke<bool>().ToArray();
                databaseWasCreated = results.First();

                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "show tables;";
                    using (var dataset = new DataTable())
                    {
                        dataset.Load(cmd.ExecuteReader());
                        totalTables = dataset.Rows.Count;
                    }
                }
            }

            // Assert
            totalTables.ShouldBe(0);
            databaseWasCreated.ShouldBeTrue();
        }
    }
}