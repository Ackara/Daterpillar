using System;
using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class MySQLSchemaAggregator : SchemaAggregatorBase
    {
        public MySQLSchemaAggregator(IDbConnection connection) : base(connection)
        {
        }

        protected override string GetColumnInfoQuery(string tableName)
        {
            return $"SELECT c.`COLUMN_NAME` AS `Name`, c.DATA_TYPE AS `Type`, if(c.CHARACTER_MAXIMUM_LENGTH IS NULL, if(c.NUMERIC_PRECISION IS NULL, 0, c.NUMERIC_PRECISION), c.CHARACTER_MAXIMUM_LENGTH) AS `Scale`, if(c.NUMERIC_SCALE IS NULL, 0, c.NUMERIC_SCALE) AS `Precision`, c.IS_NULLABLE AS `Nullable`, c.COLUMN_DEFAULT AS `Default`, if(c.EXTRA = 'auto_increment', 1, 0) AS `Auto`, c.COLUMN_COMMENT AS `Comment` FROM information_schema.`COLUMNS` c WHERE c.TABLE_SCHEMA = '{Schema.Name}' AND c.`TABLE_NAME` = '{tableName}';";
        }

        protected override string GetForeignKeyInfoQuery(string tableName)
        {
            return $"SELECT rc.`CONSTRAINT_NAME` AS `Name`, fc.FOR_COL_NAME AS `Column`, rc.REFERENCED_TABLE_NAME AS `Referecne_Table`, fc.REF_COL_NAME AS `Reference_Column`, rc.UPDATE_RULE AS `On_Update`, rc.DELETE_RULE AS `On_Delete`, rc.MATCH_OPTION AS `On_Match` FROM information_schema.REFERENTIAL_CONSTRAINTS rc JOIN information_schema.INNODB_SYS_FOREIGN_COLS fc ON fc.ID = concat(rc.`CONSTRAINT_SCHEMA`, '/', rc.`CONSTRAINT_NAME`) WHERE rc.`CONSTRAINT_SCHEMA` = '{Schema.Name}' AND rc.`TABLE_NAME` = '{tableName}';";
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
            return $"SELECT t.TABLE_NAME AS `Name`, t.TABLE_COMMENT AS `Comment` FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA = '{Schema.Name}';";
        }
    }
}