using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Ackara.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsData.Import, "SQLData")]
    public class ImportSQLDataCmdlet : Cmdlet
    {

        protected override void BeginProcessing()
        {

            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        protected override void ProcessRecord()
        {
            
        }

        protected override void EndProcessing()
        {
            _connection?.Dispose();
        }

        #region Private Members

        private const string defaultArgs = "default", explictArgs = "explict";
        private IDbConnection _connection;

        private string GetHost(string connectionString)
        {
            var builder = new SqlConnectionStringBuilder(connectionString);
            return builder.DataSource;
        }

        private void BuildConnectionString(out string connectionString)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                string dataSource = Path.IsPathRooted(Host) ? "Data Source" : "server";
                var builder = new DbConnectionStringBuilder
                {
                    { dataSource, Host },
                    { "user", User },
                    { "password", Password },
                    { "database", Database }
                };
                connectionString = builder.ConnectionString;
            }
            else { connectionString = ConnectionString; }
        }

        #endregion Private Members
    }
}
