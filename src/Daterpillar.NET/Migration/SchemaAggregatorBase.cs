using Acklann.Daterpillar.TextTransformation;
using System;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Migration
{
    public abstract class SchemaAggregatorBase : ISchemaAggregator
    {
        public SchemaAggregatorBase(IDbConnection connection)
        {
            _connection = connection;
        }

        protected Schema Schema
        {
            get { return _schema; }
        }

        protected IDbConnection Connection
        {
            get { return _connection; }
        }

        public Schema FetchSchema()
        {
            _schema = new Schema();
            _schema.Name = _connection.Database;

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
                _connection?.Dispose();
            }
        }

        protected virtual string GetTypeName(string typeName)
        {
            switch (typeName.ToLower())
            {
                case "bit":
                case "boolean":
                    return TypeNameResolverBase.BOOL;

                case "integer":
                    return TypeNameResolverBase.INT;

                case "nvarchar":
                    return TypeNameResolverBase.VARCHAR;

                default:
                    return typeName.ToLower();
            }
        }

        protected virtual void LoadColumnInformationIntoSchema(Table table, DataTable columnInfo)
        {
            foreach (DataRow row in columnInfo.Rows)
            {
                Column newColumn = table.CreateColumn();
                newColumn.Name = Convert.ToString(row[ColumnName.Name]);
                newColumn.Comment = Convert.ToString(row[ColumnName.Comment]);
                newColumn.AutoIncrement = Convert.ToBoolean(row[ColumnName.Auto]);
                newColumn.IsNullable = Convert.ToBoolean(row[ColumnName.Nullable]);
                newColumn.DataType = new DataType(GetTypeName(Convert.ToString(row[ColumnName.Type])), Convert.ToInt32(row[ColumnName.Scale]), Convert.ToInt32(row[ColumnName.Precision]));

                var columnDefault = row[ColumnName.Default];
                newColumn.DefaultValue = (columnDefault == DBNull.Value ? null : newColumn.DefaultValue);
            }
        }

        protected virtual void LoadIndexInformationIntoSchema(Table table, DataTable indexInfo)
        {
            string autoColumn = ((from x in table.Columns where x.AutoIncrement select x.Name).FirstOrDefault());

            foreach (DataRow row in indexInfo.Rows)
            {
                bool shouldAddIndex = true;

                Index newIndex = new Index();
                newIndex.Name = Convert.ToString(row[ColumnName.Name]);
                newIndex.Type = (Convert.ToString(row[ColumnName.Type])).ToIndexType();
                newIndex.Unique = Convert.ToBoolean(row[ColumnName.Unique]);
                newIndex.TableRef = table;
                shouldAddIndex = !string.IsNullOrEmpty(newIndex.Name);

                // Find and load the index columns
                using (var command = _connection.CreateCommand())
                {
                    command.CommandText = GetQueryThatFindsAllColumnsInaIndex(Convert.ToString(row[ColumnName.Id]));
                    using (var results = new DataTable())
                    {
                        results.Load(command.ExecuteReader());
                        foreach (DataRow nestedRow in results.Rows)
                        {
                            string name = Convert.ToString(nestedRow[ColumnName.Name]);

                            if (name == autoColumn)
                            {
                                shouldAddIndex = false;
                                break;
                            }
                            newIndex.Columns.Add(new IndexColumn()
                            {
                                Name = name,
                                Order = (SortOrder)Enum.Parse(typeof(SortOrder), Convert.ToString(nestedRow[ColumnName.Order]))
                            });
                        }
                    }
                }

                if (shouldAddIndex) table.Indexes.Add(newIndex);
            }
        }

        protected virtual void LoadForeignKeyInformationIntoSchema(Table table, DataTable foreignKeyInfo)
        {
            foreach (DataRow row in foreignKeyInfo.Rows)
            {
                ForeignKey newForeignKey = table.CreateForeignKey();
                newForeignKey.Name = Convert.ToString(row[ColumnName.Name]);
                newForeignKey.LocalColumn = Convert.ToString(row[ColumnName.LocalColumn]);
                newForeignKey.ForeignTable = Convert.ToString(row[ColumnName.ForeignTable]);
                newForeignKey.ForeignColumn = Convert.ToString(row[ColumnName.ForeignColumn]);
                newForeignKey.OnDelete = (Convert.ToString(row[ColumnName.OnDelete])).ToEnum();
                newForeignKey.OnUpdate = (Convert.ToString(row[ColumnName.OnUpdate])).ToEnum();
            }
        }

        #region Abstract Methods

        protected abstract string GetQueryThatFindAllTables();

        protected abstract string GetQueryThatFindsAllIndexesInaTable(string tableName);

        protected abstract string GetQueryThatFindsAllColumnsInaIndex(string indexIdentifier);

        protected abstract string GetQueryThatFindsAllColumnsInaTable(string tableName);

        protected abstract string GetQueryThatFindsAllForeignKeysInaTable(string tableName);

        #endregion Abstract Methods

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
                command.CommandText = GetQueryThatFindAllTables();
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        var newTable = _schema.CreateTable();
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
                command.CommandText = GetQueryThatFindsAllColumnsInaTable(table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadColumnInformationIntoSchema(table, results);
                }
            }
        }

        private void FetchForeignKeyInformation(Table table)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindsAllForeignKeysInaTable(table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadForeignKeyInformationIntoSchema(table, results);
                }
            }
        }

        private void FetchIndexInformation(Table table)
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindsAllIndexesInaTable(table.Name);
                System.Diagnostics.Debug.WriteLine(command.CommandText);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadIndexInformationIntoSchema(table, results);
                }
            }
        }

        #endregion Private Members

        #region ColumnNames

        protected struct ColumnName
        {
            public const string Id = "Id";
            public const string Key = "Key";
            public const string Type = "Type";
            public const string Name = "Name";
            public const string Auto = "Auto";
            public const string Order = "Order";
            public const string Scale = "Scale";
            public const string Unique = "Unique";
            public const string Comment = "Comment";
            public const string Default = "Default";
            public const string OnMatch = "On_Match";
            public const string Nullable = "Nullable";
            public const string OnUpdate = "On_Update";
            public const string OnDelete = "On_Delete";
            public const string LocalColumn = "Column";
            public const string Precision = "Precision";
            public const string ForeignTable = "Referecne_Table";
            public const string ForeignColumn = "Reference_Column";
        }

        #endregion ColumnNames
    }
}