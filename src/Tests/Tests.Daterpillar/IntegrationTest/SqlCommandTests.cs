using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.SampleSchema)]
    [DeploymentItem(Artifact.TSqlSampleSchema)]
    public class SqlCommandTests
    {
        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            try
            {
                var settings = new SqlTemplateSettings()
                {
                    AddScript = true,
                    UseDatabase = true,
                    CreateSchema = true,
                    CommentsEnabled = true,
                    DropDatabaseIfExist = false,
                };

                var schema = Schema.Load(SampleData.GetFile(Artifact.SampleSchema).OpenRead());
                var script = new SqlTemplate(settings).Transform(schema);

                using (var connection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["mssql"].ConnectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = script;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (System.Data.Common.DbException) { }
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Execute_should_retrieve_query_results_from_sql_server_database()
        {
            // Arrange
            int limit = 100;
            var query = new Query(QueryStyle.TSQL).SelectAll().Top(limit).From(Song.Table);

            IList<Song> album;
            Song track1, single;

            var sut = new AdoNetConnectionWrapper(CreateConnection(true));

            // Act
            try
            {
                album = new List<Song>(sut.Execute<Song>(query));
                track1 = album.First();

                single = sut.Execute<Song>(track1.ToSelectCommand())
                    .First();
            }
            finally
            {
                sut.Dispose();
            }

            // Assert
            Assert.AreEqual(limit, album.Count);
            Assert.AreEqual(track1.Id, single.Id);
        }

        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Insert_should_commit_add_a_new_record_to_a_sql_server_database()
        {
            // Arrange
            Song song;
            var nameOfTrack = nameof(Insert_should_commit_add_a_new_record_to_a_sql_server_database);
            var track1 = SampleData.CreateSong(nameOfTrack);
            var query = new Query().SelectAll().From($"[zune].[dbo].[{Song.Table}]")
                .Where($"{Song.NameColumn}='{track1.Name}'");

            var sut = new AdoNetConnectionWrapper(CreateConnection(true), QueryStyle.TSQL);
            System.Diagnostics.Debug.WriteLine(query.GetValue());
            // Act
            try
            {
                sut.Insert(track1);
                sut.Commit();

                song = sut.Execute<Song>(query)
                    .First();
            }
            finally
            {
                sut.Dispose();
            }

            // Assert
            Assert.AreEqual(nameOfTrack, song.Name);
            Assert.AreNotEqual(track1.Id, song.Id);
        }
        
        [TestMethod]
        [Owner(Dev.Ackara)]
        [TestCategory(Category.Integration)]
        public void Delete_should_remove_a_old_record_from_a_sql_server_database()
        {
            // Arrange
            Song insertedSong;
            bool trackDeleted;
            var track1 = SampleData.CreateSong();

            var query = new Query().SelectAll().From($"[zune].[dbo].[{Song.Table}]")
                .Where($"{Song.NameColumn}={track1.Name.ToSQL()}");

            var sut = new AdoNetConnectionWrapper(CreateConnection(true), QueryStyle.TSQL);
            System.Diagnostics.Debug.WriteLine(query.GetValue());
            // Act
            try
            {
                sut.Insert(track1);
                sut.Commit();

                insertedSong = sut.Execute<Song>(query)
                    .First();

                sut.Delete(insertedSong);
                sut.Commit();

                trackDeleted = sut.Execute<Song>(query).FirstOrDefault() == null;
            }
            finally
            {
                sut.Dispose();
            }

            // Assert
            Assert.IsNotNull(insertedSong);
            Assert.IsTrue(trackDeleted);
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

        private static IDbConnection CreateConnection(bool selectDatabase = false)
        {
            string connStr = ConfigurationManager.ConnectionStrings["mssql"].ConnectionString;
            var conn = new System.Data.SqlClient.SqlConnection(connStr);
            if (selectDatabase)
            {
                conn.Open();
                conn.ChangeDatabase("zune");
            }

            return conn;
        }

        #endregion Private Members
    }
}