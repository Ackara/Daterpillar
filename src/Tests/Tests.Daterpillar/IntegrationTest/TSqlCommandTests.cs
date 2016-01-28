using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{
    [TestClass]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.SampleSchema)]
    public class TSqlCommandTests
    {
        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.FetchData{TEntity}(string)"/> can retrieve a
        /// dataset from a SQL Server database.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunQueryOnSqlServerConnection()
        {
            // Arrange
            Song track1;
            bool allTracksHaveName = false;
            int limit = 100, trackCount = 0;

            var query = new Query(QueryStyle.TSQL)
                .SelectAll()
                .From(Song.Table)
                .Where($"{Song.IdColumn}<='{limit}'");

            // Act
            using (var connection = new AdoNetConnectionWrapper(DbFactory.CreateSqlServerConnection(), QueryStyle.TSQL))
            {
                var albums = connection.Execute<Song>(query);

                allTracksHaveName = albums.Count(x => string.IsNullOrEmpty(x.Name)) == 0;
                track1 = albums.FirstOrDefault();
            }

            // Assert
            Assert.AreEqual(limit, trackCount);
            Assert.IsTrue(allTracksHaveName);
            Assert.IsNotNull(track1);
        }

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute an insert command on a
        /// SQL Server connection.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunInsertCommandOnSqlServerConnection()
        {
        }

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute a delete command from a
        /// SQL Server database.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RunDeleteCommandOnSqlServerConnection()
        {
        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.ExceptionHandlerDelegate"/> is invoked when an
        /// exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void HandleSqlServerException()
        {
        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.Error"/> is raised when an exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void RaiseErrorEvent()
        {
        }

        #region Private Members

        private static bool _exceptionHandlerCalled, _eventRaised;

        private void ExceptionHandler(Exception ex, string command, out bool handled)
        {
            _exceptionHandlerCalled = (command == nameof(HandleSqlServerException));
            handled = true;
        }

        private void Connection_Error(object sender, DbExceptionEventArgs e)
        {
            _eventRaised = true;
        }

        #endregion Private Members
    }
}