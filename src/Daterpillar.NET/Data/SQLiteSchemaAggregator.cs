using System;
using System.Data;
using Gigobyte.Daterpillar.Transformation;
using System.Text.RegularExpressions;

namespace Gigobyte.Daterpillar.Data
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
                string temp;

                string typeName = Convert.ToString(row[ColumnName.Type]);
                typeName = (typeName == "INTEGER" ? "int" : typeName.ToLower());

                temp = _dataTypeRegex.Match(typeName)?.Groups["scale"]?.Value;
                int scale = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                temp = _dataTypeRegex.Match(typeName)?.Groups["precision"]?.Value;
                int precision = Convert.ToInt32((string.IsNullOrEmpty(temp) ? "0" : temp));

                string defaultValue = Convert.ToString(row["dflt_value"]);

                var newColumn = new Column();
                newColumn.Name = Convert.ToString(row[ColumnName.Name]);
                newColumn.DataType = new DataType(typeName, scale, precision);
                //newColumn.AutoIncrement = Convert.ToBoolean(row[ColumnName.Auto]);
                newColumn.IsNullable = !Convert.ToBoolean(row["notnull"]);
                if (!string.IsNullOrEmpty(defaultValue)) newColumn.Modifiers.Add(defaultValue);

                table.Columns.Add(newColumn);
            }
        }

        protected override void LoadForeignKeyInformationIntoSchema(Table table, DataTable foreignKeyInfo)
        {
            base.LoadForeignKeyInformationIntoSchema(table, foreignKeyInfo);
        }

        protected override string GetColumnInfoQuery(string tableName)
        {
            return $"PRAGMA table_info('{tableName}');";
        }

        protected override string GetForeignKeyInfoQuery(string tableName)
        {
            return $"PRAGMA foreign_key_list('{tableName}');";
        }

        protected override string GetIndexColumnsQuery(string indexIdentifier)
        {
            throw new NotImplementedException();
        }

        protected override string GetIndexInfoQuery(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override string GetTableInfoQuery()
        {
            return $"select sm.tbl_name AS [Name], '' AS [Comment] from sqlite_master sm WHERE sm.sql IS NOT NULL AND sm.name <> 'sqlite_sequence' AND sm.type = 'table';";
        }

        #region Private Members
        private Regex _dataTypeRegex = new Regex(@"\((?<scale>\d+),? ?(?<precision>\d+)\)", RegexOptions.Compiled);
        #endregion
    }
}