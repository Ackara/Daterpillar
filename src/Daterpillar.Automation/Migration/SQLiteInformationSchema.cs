using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provides functions to create a <see cref="Schema"/> instance using a SQL server "information schema" table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Migration.InformationSchemaBase" />
    public class SQLiteInformationSchema : InformationSchemaBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SQLiteInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public SQLiteInformationSchema(IDbConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Loads the columns.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="columnData">The column data.</param>
        internal override void LoadColumns(Table table, DataTable columnData)
        {
            foreach (DataRow row in columnData.Rows)
            {
                string typeName = _dataTypePattern.Match(Convert.ToString(row[ColumnName.Type])).Groups["type"]?.Value;
                string temp = _dataTypePattern.Match(Convert.ToString(row[ColumnName.Type]))?.Groups["scale"]?.Value;
                int scale = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                temp = _dataTypePattern.Match(Convert.ToString(row[ColumnName.Type]))?.Groups["precision"]?.Value;
                int precision = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                string defaultValue = Convert.ToString(row["dflt_value"]);

                var column = new Column()
                {
                    Table = table,
                    Name = Convert.ToString(row[ColumnName.Name]),
                    DataType = new DataType(GetTypeName(typeName), scale, precision),
                    IsNullable = !Convert.ToBoolean(row["notnull"]),
                    DefaultValue = string.IsNullOrEmpty(defaultValue) ? null : defaultValue
                };
                table.Columns.Add(column);
            }
        }

        /// <summary>
        /// Loads the foreign key.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="foreignKeyData">The foreign key data.</param>
        internal override void LoadForeignKey(Table table, DataTable foreignKeyData)
        {
            foreach (DataRow row in foreignKeyData.Rows)
            {
                var newForeignKey = new ForeignKey()
                {
                    Table = table,
                    LocalColumn = Convert.ToString(row["from"]),
                    ForeignTable = Convert.ToString(row["table"]),
                    ForeignColumn = Convert.ToString(row["to"]),
                    OnDelete = (Convert.ToString(row["on_delete"])).ToReferentialAction(),
                    OnUpdate = (Convert.ToString(row["on_update"])).ToReferentialAction()
                };
            }
        }

        /// <summary>
        /// Loads the index.
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="indexData">The index data.</param>
        internal override void LoadIndex(Table table, DataTable indexData)
        {
            string autoColumn = ((from x in table.Columns where x.AutoIncrement select x.Name).FirstOrDefault());

            foreach (DataRow row in indexData.Rows)
            {
                bool shouldInsertIndex = true;

                Index newIndex = new Index();
                newIndex.Name = Convert.ToString(row[ColumnName.Name]);
                newIndex.Type = (Convert.ToString(row["origin"]) == "pk" ? IndexType.PrimaryKey : IndexType.Index);
                newIndex.IsUnique = Convert.ToBoolean(row[ColumnName.Unique]);

                // Find and load the index columns
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = GetQueryThatFindsAllColumnsInaIndex(table.Schema.Name, Convert.ToString(row[ColumnName.Name]));
                    using (var results = new DataTable())
                    {
                        var columns = new List<Daterpillar.ColumnName>();
                        results.Load(command.ExecuteReader());
                        foreach (DataRow nestedRow in results.Rows)
                        {
                            string name = Convert.ToString(nestedRow[ColumnName.Name]);

                            if (string.IsNullOrWhiteSpace(name)) continue;
                            else if (name == autoColumn)
                            {
                                shouldInsertIndex = false;
                                break;
                            }

                            columns.Add(new Daterpillar.ColumnName()
                            {
                                Name = name,
                                Order = (Convert.ToBoolean(nestedRow["desc"]) ? Order.Descending : Order.Ascending)
                            });
                        }
                        newIndex.Columns = columns.ToArray();
                    }
                }

                if (shouldInsertIndex)
                {
                    table.Indexes.Add(newIndex);
                    newIndex.Table = table;
                }
            }
        }

        /// <summary>
        /// Gets the query that find all tables.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindAllTables(string schemaName)
        {
            return $"SELECT [sm].[tbl_name] AS [Name], '' AS [Comment] FROM [sqlite_master] sm WHERE sm.[sql] IS NOT NULL AND sm.[name] <> 'sqlite_sequence' AND sm.[type] = 'table';";
        }

        /// <summary>
        /// Gets the query that finds all columns in a index.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="indexIdentifier">The index identifier.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaIndex(string schemaName, string indexIdentifier)
        {
            return $"PRAGMA index_xinfo('{indexIdentifier}');";
        }

        /// <summary>
        /// Gets the query that finds all columns in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaTable(string schemaName, string tableName)
        {
            return $"PRAGMA table_info('{tableName}');";
        }

        /// <summary>
        /// Gets the query that finds all foreign keys in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllForeignKeysInaTable(string schemaName, string tableName)
        {
            return $"PRAGMA foreign_key_list('{tableName}');";
        }

        /// <summary>
        /// Gets the query that finds all indexes in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllIndexesInaTable(string schemaName, string tableName)
        {
            return $"PRAGMA index_list('{tableName}');";
        }

        #region Private Members

        /// <summary>
        /// The data type pattern
        /// </summary>
        private Regex _dataTypePattern = new Regex(@"(?<type>\w+)(\((?<scale>\d+)\)|\((?<scale>\d+),\s(?<precision>\d+)\))?", RegexOptions.Compiled);

        #endregion Private Members
    }
}