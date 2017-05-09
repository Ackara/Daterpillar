using System.Data;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provides functions to create a <see cref="Schema"/> instance using a SQL server "information schema" table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Migration.InformationSchemaBase" />
    public class MySQLInformationSchema : InformationSchemaBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MySQLInformationSchema"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MySQLInformationSchema(IDbConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Gets the query that find all tables.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindAllTables(string schemaName)
        {
            return $"SELECT t.TABLE_NAME AS `Name`, t.TABLE_COMMENT AS `Comment` FROM information_schema.TABLES t WHERE t.TABLE_SCHEMA = '{schemaName}';";
        }

        /// <summary>
        /// Gets the query that finds all columns in a index.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="indexIdentifier">The index identifier.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaIndex(string schemaName, string indexIdentifier)
        {
            string[] values = indexIdentifier.Split(':');
            return $"SELECT s.COLUMN_NAME AS `Name`, 'ASC' AS `Order` FROM information_schema.STATISTICS s WHERE s.TABLE_SCHEMA = '{schemaName}' AND s.TABLE_NAME = '{values[0]}' AND s.INDEX_NAME = '{values[1]}';";
        }

        /// <summary>
        /// Gets the query that finds all columns in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaTable(string schemaName, string tableName)
        {
            return $"SELECT c.`COLUMN_NAME` AS `Name`, c.DATA_TYPE AS `Type`, if(c.CHARACTER_MAXIMUM_LENGTH IS NULL, if(c.NUMERIC_PRECISION IS NULL, 0, c.NUMERIC_PRECISION), c.CHARACTER_MAXIMUM_LENGTH) AS `Scale`, if(c.NUMERIC_SCALE IS NULL, 0, c.NUMERIC_SCALE) AS `Precision`, if(c.IS_NULLABLE = 'NO', 0, 1) AS `Nullable`, c.COLUMN_DEFAULT AS `Default`, if(c.EXTRA = 'auto_increment', 1, 0) AS `Auto`, c.COLUMN_COMMENT AS `Comment` FROM information_schema.`COLUMNS` c WHERE c.TABLE_SCHEMA = '{schemaName}' AND c.`TABLE_NAME` = '{tableName}';";
        }

        /// <summary>
        /// Gets the query that finds all foreign keys in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllForeignKeysInaTable(string schemaName, string tableName)
        {
            return $"SELECT rc.`CONSTRAINT_NAME` AS `Name`, fc.FOR_COL_NAME AS `Column`, rc.REFERENCED_TABLE_NAME AS `{ColumnName.ForeignTable}`, fc.REF_COL_NAME AS `Reference_Column`, rc.UPDATE_RULE AS `On_Update`, rc.DELETE_RULE AS `On_Delete`, rc.MATCH_OPTION AS `On_Match` FROM information_schema.REFERENTIAL_CONSTRAINTS rc JOIN information_schema.INNODB_SYS_FOREIGN_COLS fc ON fc.ID = concat(rc.`CONSTRAINT_SCHEMA`, '/', rc.`CONSTRAINT_NAME`) WHERE rc.`CONSTRAINT_SCHEMA` = '{schemaName}' AND rc.`TABLE_NAME` = '{tableName}';";
        }

        /// <summary>
        /// Gets the query that finds all indexes in a table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllIndexesInaTable(string schemaName, string tableName)
        {
            return $"SELECT s.INDEX_NAME AS `Name`, if(tc.CONSTRAINT_TYPE <> 'PRIMARY KEY' OR tc.CONSTRAINT_TYPE IS NULL, 'index', 'primaryKey') AS `Type`, if(tc.CONSTRAINT_TYPE = 'UNIQUE', 1, 0) AS `Unique`, concat(s.`TABLE_NAME`, ':', s.INDEX_NAME) AS `Id` FROM information_schema.STATISTICS s LEFT JOIN information_schema.TABLE_CONSTRAINTS tc ON tc.`CONSTRAINT_NAME` = s.INDEX_NAME AND tc.TABLE_SCHEMA = s.INDEX_SCHEMA AND tc.`TABLE_NAME` = s.`TABLE_NAME` AND tc.CONSTRAINT_TYPE <> 'FOREIGN KEY' WHERE s.INDEX_SCHEMA = '{schemaName}' AND s.`TABLE_NAME` = '{tableName}' GROUP BY s.INDEX_NAME, s.`TABLE_NAME`, tc.`CONSTRAINT_TYPE`;";
        }
    }
}