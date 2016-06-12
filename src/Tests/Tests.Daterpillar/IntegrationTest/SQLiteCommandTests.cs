using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.SampleSchema)]
    [DeploymentItem(Artifact.x86SQLiteInterop)]
    [DeploymentItem(Artifact.x64SQLiteInterop)]
    public class SQLiteCommandTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            // Create SQLite database
            string path = SampleData.GetFile(Artifact.SampleSchema).FullName;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var schema = Schema.Load(stream);
                string sqlite = new SQLiteTemplate().Transform(schema);

                using (var connection = DbFactory.CreateSQLiteConnection(sqlite))
                {
                    var builder = new SQLiteConnectionStringBuilder();
                    builder.DataSource = connection.FileName;

                    _connectionString = builder.ConnectionString;
                }
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Execute_should_retrieve_query_results_from_a_sqlite_database()
        {
            // Arrange
            int limit = 200;

            using (var sut = new AdoNetConnectionWrapper(new SQLiteConnection(_connectionString), QueryStyle.SQLite))
            {
                var query = new Query()
                    .SelectAll()
                    .From(Song.Table)
                    .Limit(limit);

                // Act
                var album = sut.Execute<Song>(query);
                var track1 = album.First();

                var single = sut.Execute<Song>(track1.ToSelectCommand())
                    .First();

                // Assert
                Assert.AreEqual(limit, album.Count());
                Assert.AreEqual(track1.Id, single.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Commit_should_execute_an_insert_command_against_sqlite_a_database()
        {
            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var sut = new AdoNetConnectionWrapper(new SQLiteConnection(_connectionString), QueryStyle.SQLite))
            {
                // Act
                sut.Insert(track1);
                sut.Commit();

                var song = sut.Execute<Song>(query)
                    .First();

                // Assert
                Assert.AreEqual(nameof(Commit_should_execute_an_insert_command_against_sqlite_a_database), song.Name);
                Assert.AreNotEqual(track1.Id, song.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Commit_should_execute_a_delete_command_against_sqlite_a_database()
        {
            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var sut = new AdoNetConnectionWrapper(new SQLiteConnection(_connectionString), QueryStyle.SQLite))
            {
                // Act
                sut.Insert(track1);
                sut.Commit();

                var insertedTrack = sut.Execute<Song>(query)
                    .First();

                sut.Delete(insertedTrack);
                sut.Commit();

                bool trackDeleted = sut.Execute<Song>(query).FirstOrDefault() == null;

                // Assert
                Assert.IsNotNull(insertedTrack);
                Assert.IsTrue(trackDeleted);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void ExceptionHandler_should_call_the_method_assigned_when_an_exception_is_thrown()
        {
            // Arrange
            using (var sut = new AdoNetConnectionWrapper(new SQLiteConnection(_connectionString), QueryStyle.SQLite))
            {
                sut.ExceptionHandler = ExceptionHandler;

                // Act
                sut.ExecuteNonQuery("Invalid SQL command.");
                sut.Commit();
            }

            // Assert
            Assert.IsTrue(_exceptionHandlerCalled);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Commit_should_raise_the_error_event_when_an_exception_is_thrown()
        {
            // Arrange
            using (var connection = new AdoNetConnectionWrapper(new SQLiteConnection(_connectionString), QueryStyle.SQLite))
            {
                // Act
                connection.Error += Connection_Error;
                connection.ExecuteNonQuery("Invalid SQL command.");

                try
                {
                    connection.Commit();
                    Assert.Fail("Exception was not thrown.");
                }
                catch
                {
                    System.Threading.Thread.Sleep(500);

                    // Assert
                    Assert.IsTrue(_eventRaised);
                }
            }
        }

        #region Private Members

        private static string _connectionString;
        private static bool _exceptionHandlerCalled, _eventRaised;

        private void ExceptionHandler(Exception ex, string command, out bool handled)
        {
            _exceptionHandlerCalled = (command == nameof(ExceptionHandler_should_call_the_method_assigned_when_an_exception_is_thrown));
            handled = true;
        }

        private void Connection_Error(object sender, DbExceptionEventArgs e)
        {
            _eventRaised = true;
        }

        #endregion Private Members
    }
}