using Gigobyte.Daterpillar.Transformation;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Gigobyte.Daterpillar.Data
{
    public abstract class SchemaAggregatorBase : ISchemaAggregator
    {
        public SchemaAggregatorBase(IDbConnection connection)
        {
            _connection = connection;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public Schema FetchSchema()
        {
            Schema = new Schema();

            OpenConnectionIfClosed();
            FetchTableInformation();

            return Schema;
        }

        protected Schema GetSchema()
        {
            return Schema;
        }

        protected IDbConnection GetConnection()
        {
            return _connection;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _connection?.Dispose();
            }
        }

        protected abstract string GetTableInfoQuery();

        protected abstract string GetColumnInfoQuery(string tableName);

        #region Private Members

        private string ConnectionString;

        private IDbConnection _connection;

        private Schema Schema;

        private void OpenConnectionIfClosed()
        {
            if (_connection != null) _connection.Dispose();

            _connection = new SqlConnection(ConnectionString);
            if (_connection.State != ConnectionState.Open) _connection.Open();
        }

        private void FetchTableInformation()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = GetTableInfoQuery();
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        var newTable = new Table();
                        newTable.Name = Convert.ToString(row[ColumnName.Name]);
                        newTable.Comment = Convert.ToString(row[ColumnName.Comment]);

                        FetchColumnInformation(newTable);
                        FetchIndexInformation(newTable);
                        FetchForeignKeyInformation(newTable);
                    }
                }
            }
        }

        private void FetchColumnInformation(Table table)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = GetColumnInfoQuery(table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        var newColumn = new Column();
                        newColumn.Name = Convert.ToString(row[ColumnName.Name]);
                        newColumn.Comment = Convert.ToString(row[ColumnName.Comment]);
                        newColumn.AutoIncrement = Convert.ToBoolean(row[ColumnName.Auto]);
                    }
                }
            }
        }

        private void FetchIndexInformation(Table table)
        {
            throw new System.NotImplementedException();
        }

        private void FetchForeignKeyInformation(Table table)
        {
            throw new System.NotImplementedException();
        }

        #endregion Private Members

        #region Constants

        public struct ColumnName
        {
            public const string Name = "Name";
            public const string Auto = "Auto";
            public const string Scale = "Scale";
            public const string DataType = "Type";
            public const string Comment = "Comment";
            public const string Nullable = "Nullable";
            public const string Precision = "Precision";
        }

        #endregion Constants
    }
}