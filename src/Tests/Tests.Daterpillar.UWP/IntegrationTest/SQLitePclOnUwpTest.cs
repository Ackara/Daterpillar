using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Linq;
using System.Threading.Tasks;
using Tests.Daterpillar.Sample;
using Windows.Storage;

namespace Tests.Daterpillar.UWP.IntegrationTest
{
    [TestClass]
    public class SQLitePclOnUwpTest
    {
        [ClassInitialize]
        public static async Task Setup(TestContext context)
        {
            // Copy SQLite database
            string fileName = "sample.db";
            var dbFile = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFileAsync(fileName);
            var destination = await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, fileName, NameCollisionOption.ReplaceExisting);
            _connectionString = destination.Path;
        }

        /// <summary>
        /// Assert <see cref="SQLiteConnectionWrapper.FetchData{TEntity}(string)"/> can retrieve a dataset on a universal app.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunQueryUsingSQLiteConnectionOnUWP()
        {
            // Arrange
            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                int limit = 100;
                var query = new Query()
                    .SelectAll()
                    .From(Song.Table)
                    .Limit(limit);

                // Act
                var album = connection.Execute<Song>(query);

                // Assert
                Assert.AreEqual(limit, album.Count());
            }
        }

        /// <summary>
        /// Assert <see cref="SQLiteConnectionWrapper.Commit"/> can execute an insert command on a universal app.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunInsertCommandUsingSQLiteConnectionOnUWP()
        {
            // Arrange
            var track1 = Samples.GetSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                // Act
                connection.Insert(track1);
                connection.Commit();

                var song = connection.Execute<Song>(query)
                    .First();

                // Assert
                Assert.AreEqual(nameof(RunInsertCommandUsingSQLiteConnectionOnUWP), song.Name);
                Assert.AreNotEqual(track1.Id, song.Id);
            }
        }

        /// <summary>
        /// Assert <see cref="SQLiteConnectionWrapper.Commit"/> can execute a delete command on a universal app.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunDeleteCommandUsingSQLiteConnectionOnUWP()
        {
            // Arrange
            var track1 = Samples.GetSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                // Act
                connection.Insert(track1);
                connection.Commit();

                var insertedTrack = connection.Execute<Song>(query)
                    .First();

                connection.Delete(insertedTrack);
                connection.Commit();

                bool trackDeleted = connection.Execute<Song>(query).FirstOrDefault() == null;

                // Assert
                Assert.IsNotNull(insertedTrack);
                Assert.IsTrue(trackDeleted);
            }
        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.ExceptionHandlerDelegate"/> is invoked when an exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void HandleSQLiteExceptionOnUWP()
        {
            // Arrange
            var invalidCommand = nameof(HandleSQLiteExceptionOnUWP);

            using (var sut = new SQLiteConnectionWrapper(_connectionString))
            {
                sut.ExceptionHandler = SQLiteExceptionHandler;

                // Act
                sut.ExecuteNonQuery(invalidCommand);
                sut.Commit();
            }

            // Assert
            Assert.IsTrue(_exceptionHandlerCalled);
        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.Error"/> is raised when an exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RaiseErrorEventOnUWP()
        {
            // Arrange
            var invalidCommand = nameof(RaiseErrorEventOnUWP);

            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                connection.Error += Connection_Error;

                // Act
                connection.ExecuteNonQuery(invalidCommand);
                try
                {
                    connection.Commit();
                    Assert.Fail("Exception was not thrown.");
                }
                catch (SQLitePCL.SQLiteException) { }
            }

            // Assert
            Assert.IsTrue(_eventRaised);
        }

        /// <summary>
        /// Assert foreign key constraints are enforced when inserting new records.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void EnforceForeignKeyConstraintsOnUWP()
        {
            // Arrange
            var track = Samples.GetSong();
            track.GenreId = 9999;
            track.AlbumId = 9999;
            track.ArtistId = 9999;

            var query = new Query()
                .SelectAll()
                .From(Sample.Song.Table)
                .Where($"{Song.NameColumn} = '{nameof(EnforceForeignKeyConstraintsOnUWP)}'");

            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                // Act
                connection.Insert(track);
                try
                {
                    connection.Commit();
                    Assert.Fail("Foreign key constraint was not enforced.");
                }
                catch
                {
                    var insertedTrack = connection.Execute<Song>(query)
                        .FirstOrDefault();

                    // Assert
                    Assert.IsNull(insertedTrack, "Foreign key constraint was not enforced.");
                }
            }
        }

        #region Private Member

        private static bool _exceptionHandlerCalled, _eventRaised;

        private static string _connectionString;

        private void SQLiteExceptionHandler(Exception ex, string command, out bool handled)
        {
            handled = _exceptionHandlerCalled = (command == nameof(HandleSQLiteExceptionOnUWP));
        }

        private void Connection_Error(object sender, DbExceptionEventArgs e)
        {
            _eventRaised = true;
        }

        #endregion Private Member
    }
}