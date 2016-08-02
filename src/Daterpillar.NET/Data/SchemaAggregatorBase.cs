using Gigobyte.Daterpillar.Transformation;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Gigobyte.Daterpillar.Data
{
    public abstract class SchemaAggregatorBase : ISchemaAggregator
    {
        public SchemaAggregatorBase(string connectionString)
        {
            ConnectionString = connectionString;
        }

        protected string ConnectionString;

        protected IDbConnection Database;

        protected Schema Schema;

        public Schema FetchSchema()
        {
            Schema = new Schema();

            OpenConnectionIfClosed();
            FetchTableInformation();

            return Schema;
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
                Database?.Dispose();
            }
        }

        protected abstract string GetTableInfoQuery();

        protected abstract string GetColumnInfoQuery(string tableName);

        #region Private Members

        private void OpenConnectionIfClosed()
        {
            if (Database != null) Database.Dispose();

            Database = new SqlConnection(ConnectionString);
            if (Database.State != ConnectionState.Open) Database.Open();
        }

        private void FetchTableInformation()
        {
            using (var command = Database.CreateCommand())
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
            using (var command = Database.CreateCommand())
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

        public struct ColumnName
        {
            public const string Name = "";
            public const string Comment = "";
            public const string Auto = "";
        }
    }
}