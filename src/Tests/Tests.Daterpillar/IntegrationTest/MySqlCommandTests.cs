using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.IO;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    //[Ignore(/* To run these test provide a connection string to a MySQL database in the app.config. */)]
    public class MySqlCommandTests
    {
        public TestContext TestContext { get; set; }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            BuildMySqlDatabase();
            ConnectionString = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void FetchData_should_retrieve_query_results_from_mysql_database()
        {
            // Arrange
            using (var database = new AdoNetConnectionWrapper(new MySqlConnection(ConnectionString), QueryStyle.MySQL))
            {
                var limit = 100;

                var query = new Query()
                    .SelectAll()
                    .From(Song.Table)
                    .Where($"{Song.IdColumn}<='{limit}'");

                // Act
                var album = database.Execute<Song>(query);
                var track1 = album.First();

                var single = database.Execute<Song>(track1.ToSelectCommand())
                    .First();

                // Assert
                Assert.AreEqual(limit, album.Count());
                Assert.AreEqual(track1.Id, single.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Commit_should_execute_a_insert_command_against_mysql_database()
        {
            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new AdoNetConnectionWrapper(new MySqlConnection(ConnectionString), QueryStyle.MySQL))
            {
                // Act
                connection.Insert(track1);
                connection.Commit();

                var song = connection.Execute<Song>(query)
                    .First();

                // Assert
                Assert.AreEqual(nameof(Commit_should_execute_a_insert_command_against_mysql_database), song.Name);
                Assert.AreNotEqual(track1.Id, song.Id);
            }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        public void Commit_should_execute_a_delete_command_against_mysql_database()
        {
            // Arrange
            var track1 = SampleData.CreateSong();
            var query = new Query()
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            using (var connection = new AdoNetConnectionWrapper(new MySqlConnection(ConnectionString), QueryStyle.MySQL))
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

        internal static string ConnectionString;

        #region Private Members

        private bool _UnableToRunTests = true;

        private static void BuildMySqlDatabase()
        {
            string path = SampleData.GetFile(Artifact.SampleSchema).FullName;
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var schema = Schema.Load(stream);
                using (var connection = DbFactory.CreateMySqlConnection(schema))
                {
                    connection.Close();
                }
            }
        }

        #endregion Private Members
    }
}