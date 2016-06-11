using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    public class SqlCommandTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var schema = Schema.Load(SampleData.GetFile(SampleData.MusicxddlXML).OpenRead());
            _connectionString = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString.Trim();
            _unableToRunTests = !SampleData.TryCreateSampleDatabase(new SqlConnection(_connectionString), schema, new SqlTemplate(new SqlTemplateSettings()
            {
                AddScript = true,
                CreateSchema = true,
                UseDatabase = true,
                CommentsEnabled = false,
                DropDatabaseIfExist = true,
            }));

            _connectionString += (_connectionString.Last() == ';' ? string.Empty : ";") + $"database={schema.Name}";
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Execute_should_retrieve_query_results_from_sql_server_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            using (var sut = new AdoNetConnectionWrapper(new SqlConnection(_connectionString), QueryStyle.TSQL))
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
        public void Commit_should_execute_a_insert_command_against_sql_server_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            var trackName = nameof(Commit_should_execute_a_insert_command_against_sql_server_database);
            var track1 = SampleData.CreateSong(trackName);
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new AdoNetConnectionWrapper(new SqlConnection(_connectionString), QueryStyle.TSQL))
            {
                // Act
                connection.Insert(track1);
                connection.Commit();

                var song = connection.Execute<Song>(query)
                    .First();

                // Assert
                Assert.AreEqual(trackName, song.Name);
                Assert.AreNotEqual(track1.Id, song.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Commit_should_execute_a_delete_command_against_sql_server_database()
        {
            IgnoreTestIfDbConnectionIsUnavailable();

            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new AdoNetConnectionWrapper(new SqlConnection(_connectionString), QueryStyle.TSQL))
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

        #region Private Members

        private static string _connectionString;
        private static bool _unableToRunTests = false;

        private static void IgnoreTestIfDbConnectionIsUnavailable()
        {
            if (_unableToRunTests)
            {
                string failureMessage = $"The {nameof(SampleData)}.{nameof(SampleData.TryCreateSampleDatabase)}() method was unable to create a sample database.";
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