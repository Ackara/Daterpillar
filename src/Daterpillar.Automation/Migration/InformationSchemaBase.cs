using System;
using System.Data;
using System.Linq;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provides basic functionality to create a <see cref="Schema"/> instance using a SQL server "information schema" table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Migration.IInformationSchema" />
    public abstract class InformationSchemaBase : IInformationSchema
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationSchemaBase"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public InformationSchemaBase(IDbConnection connection)
        {
            Connection = connection;
        }

        /// <summary>
        /// The connection to the SQL server.
        /// </summary>
        protected readonly IDbConnection Connection;

        /// <summary>
        /// Creates a <see cref="T:Ackara.Daterpillar.Schema" /> instance using the information found for the specified schema.
        /// </summary>
        /// <returns>A <see cref="T:Ackara.Daterpillar.Schema" /> instance.</returns>
        public Schema FetchSchema()
        {
            return FetchSchema(Connection.Database);
        }

        /// <summary>
        /// Creates a <see cref="Ackara.Daterpillar.Schema" /> instance using the information found for the specified schema.
        /// </summary>
        /// <param name="name">The name of the schema.</param>
        /// <returns>A <see cref="Ackara.Daterpillar.Schema" /> instance.</returns>
        public Schema FetchSchema(string name)
        {
            OpenConnectionIfClosed();

            name = (string.IsNullOrEmpty(name) ? Connection.Database : name);
            var schema = new Schema() { Name = name };
            FetchTables(schema);

            return schema;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        internal virtual string GetTypeName(string typeName)
        {
            typeName = typeName.ToLower();
            switch (typeName)
            {
                case "bit":
                case "boolean":
                    return TypeResolvers.TypeResolverBase.BOOL;

                case "integer":
                    return TypeResolvers.TypeResolverBase.INT;

                case "nvarchar":
                    return TypeResolvers.TypeResolverBase.VARCHAR;

                default:
                    return typeName;
            }
        }

        internal virtual void LoadColumns(Table table, DataTable columnData)
        {
            foreach (DataRow row in columnData.Rows)
            {
                var column = new Column();
                column.Table = table;
                column.Name = Convert.ToString(row[ColumnName.Name]);
                column.Comment = Convert.ToString(row[ColumnName.Comment]);
                column.AutoIncrement = Convert.ToBoolean(row[ColumnName.Auto]);
                column.IsNullable = Convert.ToBoolean(row[ColumnName.Nullable]);
                column.DataType = new DataType(GetTypeName(Convert.ToString(row[ColumnName.Type])), Convert.ToInt32(row[ColumnName.Scale]), Convert.ToInt32(row[ColumnName.Precision]));

                var columnDefault = row[ColumnName.Default];
                column.DefaultValue = (columnDefault == DBNull.Value ? null : column.DefaultValue);
                table.Columns.Add(column);
            }
        }

        internal virtual void LoadIndex(Table table, DataTable indexData)
        {
            string autoColumn = ((from x in table.Columns where x.AutoIncrement select x.Name).FirstOrDefault());

            foreach (DataRow row in indexData.Rows)
            {
                bool shouldAddIndex = true;

                var newIndex = new Index();
                newIndex.Table = table;
                newIndex.Name = Convert.ToString(row[ColumnName.Name]);
                newIndex.Type = (Convert.ToString(row[ColumnName.Type])).ToIndexType();
                newIndex.IsUnique = Convert.ToBoolean(row[ColumnName.Unique]);
                shouldAddIndex = !string.IsNullOrEmpty(newIndex.Name);

                // Find and load the index columns
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = GetQueryThatFindsAllColumnsInaIndex(table.Schema.Name, Convert.ToString(row[ColumnName.Id]));
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
                            newIndex.Columns = new Daterpillar.ColumnName[]
                            {
                                new Daterpillar.ColumnName()
                                {
                                    Name = name,
                                    Order = (Order)Enum.Parse(typeof(Order), Convert.ToString(nestedRow[ColumnName.Order]).ToOrder())
                                }
                            };
                        }
                    }
                }

                if (shouldAddIndex) table.Indexes.Add(newIndex);
            }
        }

        internal virtual void LoadForeignKey(Table table, DataTable foreignKeyData)
        {
            foreach (DataRow row in foreignKeyData.Rows)
            {
                var foreignKey = new ForeignKey()
                {
                    Table = table,
                    Name = Convert.ToString(row[ColumnName.Name]),
                    LocalColumn = Convert.ToString(row[ColumnName.LocalColumn]),
                    ForeignTable = Convert.ToString(row[ColumnName.ForeignTable]),
                    ForeignColumn = Convert.ToString(row[ColumnName.ForeignColumn]),
                    OnDelete = (Convert.ToString(row[ColumnName.OnDelete])).ToReferentialAction(),
                    OnUpdate = (Convert.ToString(row[ColumnName.OnUpdate])).ToReferentialAction()
                };

                table.ForeignKeys.Add(foreignKey);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Connection?.Dispose();
        }

        #region Abstract Methods

        /// <summary>
        /// Gets the query that find all tables.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>A query.</returns>
        protected abstract string GetQueryThatFindAllTables(string schemaName);

        /// <summary>
        /// Gets the query that finds all indexes in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected abstract string GetQueryThatFindsAllIndexesInaTable(string schemaName, string tableName);

        /// <summary>
        /// Gets the query that finds all columns in a index.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="indexIdentifier">The index identifier.</param>
        /// <returns>A query.</returns>
        protected abstract string GetQueryThatFindsAllColumnsInaIndex(string schemaName, string indexIdentifier);

        /// <summary>
        /// Gets the query that finds all columns in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected abstract string GetQueryThatFindsAllColumnsInaTable(string schemaName, string tableName);

        /// <summary>
        /// Gets the query that finds all foreign keys in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected abstract string GetQueryThatFindsAllForeignKeysInaTable(string schemaName, string tableName);

        #endregion Abstract Methods

        #region Private Members

        private void OpenConnectionIfClosed()
        {
            if (Connection.State != ConnectionState.Open) Connection.Open();
        }

        private void FetchTables(Schema schema)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindAllTables(schema.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    foreach (DataRow row in results.Rows)
                    {
                        var table = new Table()
                        {
                            Name = Convert.ToString(row[ColumnName.Name]),
                            Comment = Convert.ToString(row[ColumnName.Comment])
                        };

                        schema.Add(table);
                        FetchColumns(table);
                        FetchForeignKeys(table);
                        FetchIndexes(table);
                    }
                }
            }
        }

        private void FetchColumns(Table table)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindsAllColumnsInaTable(table.Schema.Name, table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadColumns(table, results);
                }
            }
        }

        private void FetchIndexes(Table table)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindsAllIndexesInaTable(table.Schema.Name, table.Name);
                System.Diagnostics.Debug.WriteLine(command.CommandText);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadIndex(table, results);
                }
            }
        }

        private void FetchForeignKeys(Table table)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = GetQueryThatFindsAllForeignKeysInaTable(table.Schema.Name, table.Name);
                using (var results = new DataTable())
                {
                    results.Load(command.ExecuteReader());
                    LoadForeignKey(table, results);
                }
            }
        }

        #endregion Private Members

        internal struct ColumnName
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
            public const string ForeignTable = "Reference_Table";
            public const string ForeignColumn = "Reference_Column";
        }
    }
}