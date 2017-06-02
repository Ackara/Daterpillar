using Acklann.Daterpillar.Cmdlets;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System;
using System.IO;
using System.Linq;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    public class RemoveDatabaseCmdletTest
    {
        [TestMethod]
        public void Invoke_should_remove_a_database_from_the_specified_server_when_a_connection_string_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmdlet_drop";
            var connectionString = ConnectionFactory.GetMySQLConnectionString();
            var sut = new RemoveDatabaseCmdlet()
            {
                ConnectionString = connectionString,
                Database = databaseName,
                Syntax = "mysql"
            };

            // Act
            using (var connection = ConnectionFactory.CreateMySQLConnection())
            {
                var scriptFailed = !connection.TryExecuteScript($"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", out string errorMsg);
                if (scriptFailed) Assert.Fail(errorMsg);
            }

            var results = sut.Invoke<bool>().ToArray();
            var databaseWasDropped = results[0];
            using (var connection = ConnectionFactory.CreateMySQLConnection(databaseName))
            {
                try
                {
                    connection.Open();
                    Assert.Fail($"The [{databaseName}] database still exist.");
                }
                catch { }
            }

            // Assert
            results.Length.ShouldBe(1);
            databaseWasDropped.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_remove_a_database_from_the_specified_server_when_a_all_args_is_passed()
        {
            // Arrange
            var databaseName = "dtpl_cmdlet_remove3";
            var connectionString = ConnectionFactory.GetMySQLConnectionString(databaseName);
            var builder = new MySql.Data.MySqlClient.MySqlConnectionStringBuilder(connectionString);
            var sut = new RemoveDatabaseCmdlet()
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
                var scriptFailed = !connection.TryExecuteScript($"CREATE DATABASE IF NOT EXISTS `{databaseName}`;", out string errorMsg);
                if (scriptFailed) Assert.Fail(errorMsg);
            }

            var results = sut.Invoke<bool>().ToArray();
            var databaseWasDropped = results[0];
            using (var connection = ConnectionFactory.CreateMySQLConnection(databaseName))
            {
                try
                {
                    connection.Open();
                    Assert.Fail($"The database was not dropped.");
                }
                catch { }
            }

            // Assert
            results.Length.ShouldBe(1);
            databaseWasDropped.ShouldBeTrue();
        }

        [TestMethod]
        public void Invoke_should_delete_a_sqlite_database_when_a_all_args_is_passed()
        {
            // Arrange
            var databaseFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{nameof(RemoveDatabaseCmdletTest)}.db3");
            var sut = new RemoveDatabaseCmdlet()
            {
                Host = databaseFilePath,
                Syntax = "sqlite"
            };

            // Act
            if (File.Exists(databaseFilePath) == false) File.Create(databaseFilePath).Dispose();
            var results = sut.Invoke<bool>().ToArray();
            var operationCompleted = results[0];
            var dbFileExist = File.Exists(databaseFilePath);

            // Assert
            results.Length.ShouldBe(1);
            dbFileExist.ShouldBeFalse();
            operationCompleted.ShouldBeTrue();
        }
    }
}