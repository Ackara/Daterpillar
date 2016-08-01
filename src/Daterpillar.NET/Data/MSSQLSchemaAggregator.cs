using Gigobyte.Daterpillar.Transformation;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Gigobyte.Daterpillar.Data
{
    public class MSSQLSchemaAggregator : ISchemaAggregator
    {
        public MSSQLSchemaAggregator()
        {
        }

        public MSSQLSchemaAggregator(string connectionString) 
        {
            ConnectionString = connectionString;
        }
        

        public string ConnectionString { get; set; }

        public Schema FetchSchema()
        {
            _schema = new Schema();

            OpenConnectionIfClosed();
            FetchTableInformation();

            return _schema;
        }

        

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _database?.Dispose();
            }
        }

        #region Private Members

        private Schema _schema;
        private IDbConnection _database;

        private void OpenConnectionIfClosed()
        {
            if (_database != null) _database.Dispose();

            _database = new SqlConnection(ConnectionString);
            if (_database.State != ConnectionState.Open) _database.Open();
        }

        private void FetchTableInformation()
        {
            string query = $"";

            using (var command = _database.CreateCommand())
            {
                command.CommandText = query;
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        _schema.Tables.Add(ConvertToTable(row));
                    }
                }
            }
        }

        private Table ConvertToTable(DataRow data)
        {
            var table = new Table();



            return table;
        }
        #endregion Private Members
    }
}