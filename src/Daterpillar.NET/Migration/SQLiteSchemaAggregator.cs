using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar.Migration
{
    public class SQLiteSchemaAggregator : SchemaAggregatorBase
    {
        public SQLiteSchemaAggregator(IDbConnection connection) : base(connection)
        {
        }

        protected override void LoadColumnInformationIntoSchema(Table table, DataTable columnInfo)
        {
            foreach (DataRow row in columnInfo.Rows)
            {
                string typeName = _dataTypeRegex.Match(Convert.ToString(row[ColumnName.Type])).Groups["type"]?.Value;

                string temp;
                temp = _dataTypeRegex.Match(Convert.ToString(row[ColumnName.Type]))?.Groups["scale"]?.Value;
                int scale = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                temp = _dataTypeRegex.Match(Convert.ToString(row[ColumnName.Type]))?.Groups["precision"]?.Value;
                int precision = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                string defaultValue = Convert.ToString(row["dflt_value"]);

                Column newColumn = table.CreateColumn();
                newColumn.Name = Convert.ToString(row[ColumnName.Name]);
                newColumn.DataType = new DataType(GetTypeName(typeName), scale, precision);
                newColumn.IsNullable = !Convert.ToBoolean(row["notnull"]);
                newColumn.DefaultValue = string.IsNullOrEmpty(defaultValue) ? null : defaultValue;
            }
        }

        protected override void LoadForeignKeyInformationIntoSchema(Table table, DataTable foreignKeyInfo)
        {
            int counter = 1;

            foreach (DataRow row in foreignKeyInfo.Rows)
            {
                ForeignKey newForeignKey = table.CreateForeignKey();
                newForeignKey.LocalColumn = Convert.ToString(row["from"]);
                newForeignKey.ForeignTable = Convert.ToString(row["table"]);
                newForeignKey.ForeignColumn = Convert.ToString(row["to"]);
                newForeignKey.OnDelete = (Convert.ToString(row["on_delete"])).ToEnum();
                newForeignKey.OnUpdate = (Convert.ToString(row["on_update"])).ToEnum();
                newForeignKey.Name = newForeignKey.GetName(counter++);
            }
        }

        protected override void LoadIndexInformationIntoSchema(Table table, DataTable indexInfo)
        {
            string autoColumn = ((from x in table.Columns where x.AutoIncrement select x.Name).FirstOrDefault());

            foreach (DataRow row in indexInfo.Rows)
            {
                bool shouldInsertIndex = true;

                Index newIndex = new Index();
                newIndex.Name = Convert.ToString(row[ColumnName.Name]);
                newIndex.Type = (Convert.ToString(row["origin"]) == "pk" ? IndexType.PrimaryKey : IndexType.Index);
                newIndex.Unique = Convert.ToBoolean(row[ColumnName.Unique]);

                // Find and load the index columns
                using (var command = Connection.CreateCommand())
                {
                    command.CommandText = GetQueryThatFindsAllColumnsInaIndex(Convert.ToString(row[ColumnName.Name]));
                    using (var results = new DataTable())
                    {
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
                            newIndex.Columns.Add(new IndexColumn()
                            {
                                Name = name,
                                Order = (Convert.ToBoolean(nestedRow["desc"]) ? SortOrder.DESC : SortOrder.ASC)
                            });
                        }
                    }
                }

                if (shouldInsertIndex)
                {
                    table.Indexes.Add(newIndex);
                    newIndex.TableRef = table;
                }
            }
        }

        protected override string GetQueryThatFindsAllColumnsInaTable(string tableName)
        {
            return $"PRAGMA table_info('{tableName}');";
        }

        protected override string GetQueryThatFindsAllForeignKeysInaTable(string tableName)
        {
            return $"PRAGMA foreign_key_list('{tableName}');";
        }

        protected override string GetQueryThatFindsAllColumnsInaIndex(string indexIdentifier)
        {
            return $"PRAGMA index_xinfo('{indexIdentifier}');";
        }

        protected override string GetQueryThatFindsAllIndexesInaTable(string tableName)
        {
            return $"PRAGMA index_list('{tableName}');";
        }

        protected override string GetQueryThatFindAllTables()
        {
            return $"SELECT [sm].[tbl_name] AS [Name], '' AS [Comment] FROM [sqlite_master] sm WHERE sm.[sql] IS NOT NULL AND sm.[name] <> 'sqlite_sequence' AND sm.[type] = 'table';";
        }

        #region Private Members

        private Regex _dataTypeRegex = new Regex(@"(?<type>\w+)(\((?<scale>\d+)\)|\((?<scale>\d+),\s(?<precision>\d+)\))?", RegexOptions.Compiled);

        #endregion Private Members
    }
}