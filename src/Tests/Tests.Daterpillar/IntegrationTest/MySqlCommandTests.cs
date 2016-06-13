using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    public class MySqlCommandTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var schema = Schema.Load(SampleData.GetFile(SampleData.MusicXddlXML).OpenRead());
            _connectionString = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString.Trim();
            _unableToRunTests = !SampleData.TryCreateSampleDatabase(new MySqlConnection(_connectionString), schema, new MySqlTemplate(new MySqlTemplateSettings()
            {
                CommentsEnabled = false,
                DropDatabaseIfExist = true
            }));

            _connectionString += (_connectionString.Last() == ';' ? string.Empty : ";") + $"database={schema.Name}";
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void FetchData_should_retrieve_query_results_from_mysql_a_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            using (var sut = new AdoNetConnectionWrapper(new MySqlConnection(_connectionString), QueryStyle.MySQL))
            {
                var limit = 100;

                var query = new Query()
                    .SelectAll()
                    .From(Song.Table)
                    .Where($"{Song.IdColumn}<='{limit}'");

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
        public void Commit_should_execute_an_insert_command_against_mysql_a_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var sut = new AdoNetConnectionWrapper(new MySqlConnection(_connectionString), QueryStyle.MySQL))
            {
                // Act
                sut.Insert(track1);
                sut.Commit();

                var song = sut.Execute<Song>(query)
                    .First();

                // Assert
                Assert.AreEqual(track1.Name, song.Name);
                Assert.AreNotEqual(track1.Id, song.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Commit_should_execute_a_delete_command_against_mysql_a_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var sut = new AdoNetConnectionWrapper(new MySqlConnection(_connectionString), QueryStyle.MySQL))
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

        #region Private Members

        private static string _connectionString;
        private static bool _unableToRunTests = false;

        private static void IgnoreTestIfDbConnectionIsUnavailable()
        {
            if (_unableToRunTests)
            {
                string failureMessage = $"The {nameof(SampleData.TryCreateSampleDatabase)}() method was unable to create a sample database.";
#if DEBUG
                Assert.Inconclusive(failureMessage);
#else
                Assert.Fail(failureMessage);
#endif
            }
        }

        #endregion Private Members
    }
}