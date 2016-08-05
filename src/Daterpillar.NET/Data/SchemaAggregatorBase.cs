using Gigobyte.Daterpillar.Transformation;
using System;
using System.Data;

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
            _schema = new Schema();

            OpenConnectionIfClosed();
            FetchTableInformation();

            return _schema;
        }

        protected Schema GetSchema()
        {
            return _schema;
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

        protected abstract string GetIndexInfoQuery(string tableName);

        protected abstract string GetColumnInfoQuery(string tableName);

        protected abstract string GetForeignKeyInfoQuery(string tableName);

        #region Private Members

        private string ConnectionString;

        private IDbConnection _connection;

        private Schema _schema;

        private void OpenConnectionIfClosed()
        {
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
                        //FetchIndexInformation(newTable);
                        FetchForeignKeyInformation(newTable);
                        _schema.Tables.Add(newTable);
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
                        string nullable = Convert.ToString(row[ColumnName.Nullable]);
                        string defaultValue = Convert.ToString(row[ColumnName.Default]);
                        string typeName = Convert.ToString(row[ColumnName.DataType]);
                        int scale = Convert.ToInt32(row[ColumnName.Scale]);
                        int precision = Convert.ToInt32(row[ColumnName.Precision]);

                        var newColumn = new Column();
                        newColumn.Name = Convert.ToString(row[ColumnName.Name]);
                        newColumn.Comment = Convert.ToString(row[ColumnName.Comment]);
                        newColumn.DataType = new DataType(typeName, scale, precision);
                        newColumn.AutoIncrement = Convert.ToBoolean(row[ColumnName.Auto]);
                        if (!string.IsNullOrEmpty(nullable)) newColumn.Modifiers.Add(nullable);
                        if (!string.IsNullOrEmpty(defaultValue)) newColumn.Modifiers.Add(defaultValue);
                        if (newColumn.AutoIncrement) newColumn.Modifiers.Add("PRIMARY KEY");

                        table.Columns.Add(newColumn);
                    }
                }
            }
        }

        private void FetchForeignKeyInformation(Table table)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = GetForeignKeyInfoQuery(table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {

                    }
                }
            }
        }

        private void FetchIndexInformation(Table table)
        {
            throw new System.NotImplementedException();
        }


        #endregion Private Members

        #region Constants

        public struct ColumnName
        {
            public const string Key = "Key";
            public const string Name = "Name";
            public const string Auto = "Auto";
            public const string Scale = "Scale";
            public const string DataType = "Type";
            public const string Comment = "Comment";
            public const string Nullable = "Nullable";
            public const string Precision = "Precision";
            public const string Default = "Default";
        }

        #endregion Constants
    }
}