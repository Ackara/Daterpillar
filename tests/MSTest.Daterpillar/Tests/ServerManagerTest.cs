using Ackara.Daterpillar.Migration;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using System.IO;

namespace MSTest.Daterpillar.Tests
{
    [TestClass]
    [DeploymentItem(FName.X86)]
    public class ServerManagerTest
    {
        // MSSQL
        [TestMethod]
        public void CreateDatabase_should_attach_a_new_mssql_database_to_the_specified_server_when_invoked()
        {
            // Arrange
            string errorMsg, dbName = "dtpl_mgr_add";
            bool databaseWasCreated, operationWasSuccessful;
            var sut = new MSSQLServerManager(ConnectionFactory.GetMSSQLConnectionString());

            // Act
            using (sut)
            {
                using (var connection = sut.GetConnection())
                {
                    try
                    {
                        var server = ConnectionFactory.AsSqlServerInstance(connection);
                        server.KillAllProcesses(dbName);
                        server.KillDatabase(dbName);
                    }
                    catch (FailedOperationException) { }
                }

                using (var connection = sut.GetConnection())
                {
                    operationWasSuccessful = sut.CreateDatabase(dbName);
                    connection.Open();
                    connection.ChangeDatabase(dbName);
                    databaseWasCreated = connection.TryExecuteScript("create table add_op (id int not null);", out errorMsg);
                }
            }

            // Assert
            operationWasSuccessful.ShouldBeTrue();
            databaseWasCreated.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void DropDatabase_should_kill_an_existing_mssql_database_from_the_specified_server_when_invoked()
        {
            // Arrange
            string dbName = "dtpl_drop_op";
            bool operationWasSuccessful, databaseDoNotExist;
            var sut = new MSSQLServerManager(ConnectionFactory.GetMSSQLConnectionString());

            // Act
            using (sut)
            {
                using (var connection = ConnectionFactory.CreateMSSQLConnection(dbName))
                {
                    connection.UseEmptyDatabase();
                    operationWasSuccessful = sut.DropDatabase(dbName);
                    try
                    {
                        connection.Open();
                        databaseDoNotExist = false;
                        Assert.Fail($"The '{dbName}' database was not removed from the server.");
                    }
                    catch { databaseDoNotExist = true; }
                }
            }

            // Assert
            databaseDoNotExist.ShouldBeTrue();
            operationWasSuccessful.ShouldBeTrue();
        }

        // MySQL

        [TestMethod]
        public void CreateDatabase_should_attach_a_new_mysql_database_to_the_specified_server_when_invoked()
        {
            // Arrange
            string errorMsg, dbName = "dtpl_mgr_add";
            bool databaseWasCreated, operationWasSuccessful;
            var sut = new MySQLServerManager(ConnectionFactory.GetMySQLConnectionString());

            // Act
            using (sut)
            {
                using (var connection = sut.GetConnection())
                {
                    connection.TryExecuteScript($"DROP DATABASE IF EXISTS `{dbName}`;", out errorMsg);
                }

                using (var connection = sut.GetConnection())
                {
                    operationWasSuccessful = sut.CreateDatabase(dbName);
                    connection.Open();
                    connection.ChangeDatabase(dbName);
                    databaseWasCreated = connection.TryExecuteScript("create table add_op (id int not null);", out errorMsg);
                }
            }

            // Assert
            operationWasSuccessful.ShouldBeTrue();
            databaseWasCreated.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void DropDatabase_should_kill_an_existing_mysql_database_from_the_specified_server_when_invoked()
        {
            // Arrange
            string dbName = "dtpl_drop_op";
            bool operationWasSuccessful, databaseDoNotExist;
            var sut = new MySQLServerManager(ConnectionFactory.GetMySQLConnectionString());

            // Act
            using (sut)
            {
                using (var connection = ConnectionFactory.CreateMySQLConnection(dbName))
                {
                    connection.UseEmptyDatabase();
                    operationWasSuccessful = sut.DropDatabase(dbName);
                    try
                    {
                        connection.Open();
                        databaseDoNotExist = false;
                        Assert.Fail($"The '{dbName}' database was not removed from the server.");
                    }
                    catch { databaseDoNotExist = true; }
                }
            }

            // Assert
            databaseDoNotExist.ShouldBeTrue();
            operationWasSuccessful.ShouldBeTrue();
        }

        // SQLite

        [TestMethod]
        public void CreateDatabase_should_attach_a_new_sqlite_database_to_the_specified_server_when_invoked()
        {
            // Arrange
            string errorMsg, dbName = "dtpl_mgr_add";
            bool databaseWasCreated, operationWasSuccessful;
            var sut = new SQLiteServerManager(ConnectionFactory.GetSQLiteConnectionString());

            // Act
            using (sut)
            {
                using (var connection = sut.GetConnection())
                {
                    if (File.Exists(sut.FilePath)) File.Delete(sut.FilePath);
                    operationWasSuccessful = sut.CreateDatabase(dbName);
                    databaseWasCreated = connection.TryExecuteScript("create table add_op (id int not null);", out errorMsg);
                }
            }

            // Assert
            File.Exists(sut.FilePath).ShouldBeTrue();
            operationWasSuccessful.ShouldBeTrue();
            databaseWasCreated.ShouldBeTrue(errorMsg);
        }

        [TestMethod]
        public void DropDatabase_should_kill_an_existing_sqlite_database_from_the_specified_server_when_invoked()
        {
            // Arrange
            string dbName = "dtpl_drop_op";
            bool operationWasSuccessful, databaseDoNotExist;
            var sut = new SQLiteServerManager(ConnectionFactory.GetSQLiteConnectionString());

            // Act
            using (sut)
            {
                using (var connection = ConnectionFactory.CreateSQLiteConnection(dbName))
                {
                    connection.UseEmptyDatabase();
                    operationWasSuccessful = sut.DropDatabase(dbName);
                    try
                    {
                        connection.Open();
                        databaseDoNotExist = false;
                        Assert.Fail($"The '{dbName}' database was not removed from the server.");
                    }
                    catch { databaseDoNotExist = true; }
                }
            }

            // Assert
            databaseDoNotExist.ShouldBeTrue();
            operationWasSuccessful.ShouldBeTrue();
        }
    }
}