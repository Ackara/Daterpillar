using Gigobyte.Daterpillar.Data;
using Gigobyte.Daterpillar.Data.Linq;
using Gigobyte.Daterpillar.Transformation;
using Gigobyte.Daterpillar.Transformation.Template;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Tests.Daterpillar.Sample;

namespace Tests.Daterpillar.IntegrationTest
{

    [Ignore]
    [TestClass]
    [DeploymentItem(Artifact.SamplesFolder + Artifact.SampleSchema)]
    public class TSqlCommandTests
    {
        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.FetchData{TEntity}(string)"/> can retrieve a dataset from a SQL Server database.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunQueryOnTSqlConnection()
        {
            // Arrange

            using (var sut = new AdoNetConnectionWrapper(DbFactory.CreateMsSqlConnection(null), QueryStyle.TSQL))
            {

            }

            // Act

            // Arrange
        }

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute an insert command on a SQL Server connection.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunInsertCommandOnTSqlConnection()
        {
        }

        /// <summary>
        /// Assert <see cref="AdoNetConnectionWrapper.Commit"/> can execute a delete command from a SQL Server database.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RunDeleteCommandOnTSqlConnection()
        {
            
        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.ExceptionHandlerDelegate"/> is invoked when an exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void HandleTSqlException()
        {

        }

        /// <summary>
        /// Assert <see cref="DbConnectionWrapperBase.Error"/> is raised when an exception is thrown.
        /// </summary>
        [TestMethod]
        [Owner(Str.Ackara)]
        public void RaiseErrorEvent()
        {

        }

        #region Private Members

        private static bool _exceptionHandlerCalled, _eventRaised;

        private void ExceptionHandler(Exception ex, string command, out bool handled)
        {
            _exceptionHandlerCalled = (command == nameof(HandleTSqlException));
            handled = true;
        }

        private void Connection_Error(object sender, DbExceptionEventArgs e)
        {
            _eventRaised = true;
        }

        #endregion Private Members
    }
}