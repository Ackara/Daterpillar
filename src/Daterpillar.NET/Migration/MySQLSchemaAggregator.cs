using System.Data;

namespace Acklann.Daterpillar.Migration
{
    public class MySQLSchemaAggregator : SchemaAggregatorBase
    {
        public MySQLSchemaAggregator(IDbConnection connection) : base(connection)
        {
        }

        protected override string GetQueryThatFindsAllColumnsInaTable(string tableName)
        {
            return $"SELECT c.`COLUMN_NAME` AS `Name`, c.DATA_TYPE AS `Type`, if(c.CHARACTER_MAXIMUM_LENGTH IS NULL, if(c.NUMERIC_PRECISION IS NULL, 0, c.NUMERIC_PRECISION), c.CHARACTER_MAXIMUM_LENGTH) AS `Scale`, if(c.NUMERIC_SCALE IS NULL, 0, c.NUMERIC_SCALE) AS `Precision`, if(c.IS_NULLABLE = 'NO', 0, 1) AS `Nullable`, c.COLUMN_DEFAULT AS `Default`, if(c.EXTRA = 'auto_increment', 1, 0) AS `Auto`, c.COLUMN_COMMENT AS `Comment` FROM information_schema.`COLUMNS` c WHERE c.TABLE_SCHEMA = '{Schema.Name}' AND c.`TABLE_NAME` = '{tableName}';";
        }

        protected override string GetQueryThatFindsAllForeignKeysInaTable(string tableName)
        {
            return $"SELECT rc.`CONSTRAINT_NAME` AS `Name`, fc.FOR_COL_NAME AS `Column`, rc.REFERENCED_TABLE_NAME AS `Referecne_Table`, fc.REF_COL_NAME AS `Reference_Column`, rc.UPDATE_RULE AS `On_Update`, rc.DELETE_RULE AS `On_Delete`, rc.MATCH_OPTION AS `On_Match` FROM information_schema.REFERENTIAL_CONSTRAINTS rc JOIN information_schema.INNODB_SYS_FOREIGN_COLS fc ON fc.ID = concat(rc.`CONSTRAINT_SCHEMA`, '/', rc.`CONSTRAINT_NAME`) WHERE rc.`CONSTRAINT_SCHEMA` = '{Schema.Name}' AND rc.`TABLE_NAME` = '{tableName}';";
        }

        protected override string GetQueryThatFindsAllColumnsInaIndex(string indexIdentifier)
        {
            string[] values = indexIdentifier.Split(':');
            return $"SELECT s.COLUMN_NAME AS `Name`, 'ASC' AS `Order` FROM information_schema.STATISTICS s WHERE s.TABLE_SCHEMA = '{Schema.Name}' AND s.TABLE_NAME = '{values[0]}' AND s.INDEX_NAME = '{values[1]}';";
        }

        protected override string GetQueryThatFindsAllIndexesInaTable(string tableName)
        {
            return $"SELECT s.INDEX_NAME AS `Name`, if(tc.CONSTRAINT_TYPE <> 'PRIMARY KEY' OR tc.CONSTRAINT_TYPE IS NULL, 'index', 'primaryKey') AS `Type`, if(tc.CONSTRAINT_TYPE = 'UNIQUE', 1, 0) AS `Unique`, concat(s.`TABLE_NAME`, ':', s.INDEX_NAME) AS `Id` FROM information_schema.STATISTICS s LEFT JOIN information_schema.TABLE_CONSTRAINTS tc ON tc.`CONSTRAINT_NAME` = s.INDEX_NAME AND tc.TABLE_SCHEMA = s.INDEX_SCHEMA AND tc.`TABLE_NAME` = s.`TABLE_NAME` AND tc.CONSTRAINT_TYPE <> 'FOREIGN KEY' WHERE s.INDEX_SCHEMA = '{Schema.Name}' AND s.`TABLE_NAME` = '{tableName}' GROUP BY s.INDEX_NAME, s.`TABLE_NAME`, tc.`CONSTRAINT_TYPE`;";
        }

        protected override string GetQueryThatFindAllTables()
        {
            return $"SELECT t.TABLE_NAME AS `Name`, t.TABLE_COMMENT AS `Comment` FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA = '{Schema.Name}';";
        }
    }
}