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
            var newDest = await dbFile.CopyAsync(ApplicationData.Current.LocalFolder, fileName, NameCollisionOption.ReplaceExisting);
            _connectionString = newDest.Path;
        }

        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunQueryUsingSQLiteConnectionOnUWP()
        {
            // Arrange
            using (var connection = new SQLiteConnectionWrapper(_connectionString))
            {
                var query = new Query()
                    .SelectAll()
                    .From(Song.Table)
                    .Where($"{Song.IdColumn}<='100'");

                // Act
                var album = connection.Execute<Song>(query);

                // Assert
                Assert.AreEqual(3, album.Count());
            }
        }

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

        [TestMethod]
        [Owner(Str.Ackara)]
        public void HandleSQLiteExceptionOnUWP()
        {
            // Arrange
            var invalidCommand = nameof(HandleSQLiteExceptionOnUWP);

            using (var sut = new SQLiteConnectionWrapper(_connectionString))
            {
                sut.ExceptionHandler = SQliteExceptionHanlder;

                // Act
                sut.ExecuteNonQuery(invalidCommand);
                sut.Commit();
            }

            // Assert
            Assert.IsTrue(_exceptionHandlerCalled);
        }

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

        #region Private Member

        private static bool _exceptionHandlerCalled, _eventRaised;

        private static string _connectionString;

        private void SQliteExceptionHanlder(Exception ex, string command, out bool handled)
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