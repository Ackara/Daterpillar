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
    [Ignore(/* To run these test provide a connection string to a SQL Server database in the app.config. */)]
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

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.FetchData{TEntity}(string)"/> can retrieve a
        /// dataset from a SQL Server database.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunQueryOnSqlConnection()
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

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute an insert command on a
        /// SQL Server connection.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunInsertCommandOnSqlConnection()
        {
            // Arrange
            Song song;
            var track1 = SampleData.CreateSong();
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
            Assert.AreEqual(nameof(RunInsertCommandOnSqlConnection), song.Name);
            Assert.AreNotEqual(track1.Id, song.Id);
        }

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute a delete command on a
        /// SQL Server connection.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunDeleteCommandOnSqlConnection()
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