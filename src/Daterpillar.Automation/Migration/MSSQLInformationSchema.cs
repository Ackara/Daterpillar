using System.Data;

namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Provides functions to create a <see cref="Schema"/> instance using a SQL server "information schema" table.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Migration.InformationSchemaBase" />
    public class MSSQLInformationSchema : InformationSchemaBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLInformationSchema" /> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public MSSQLInformationSchema(string connectionString) : this(new System.Data.SqlClient.SqlConnection(connectionString))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MSSQLInformationSchema" /> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public MSSQLInformationSchema(IDbConnection connection) : base(connection)
        {
        }

        /// <summary>
        /// Gets the query that find all tables.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindAllTables(string schemaName)
        {
            return $"SELECT [TABLE_NAME] AS [Name], '' AS[Comment] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
        }

        /// <summary>
        /// Gets the index of the query that finds all columns ina.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="indexIdentifier">The index identifier.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaIndex(string schemaName, string indexIdentifier)
        {
            string[] values = indexIdentifier.Split(':');
            return $"SELECT c.name AS [Name], CASE  WHEN ic.is_descending_key = 1 THEN 'DESC'  ELSE 'ASC' END AS [Order] FROM sys.index_columns ic JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id WHERE ic.object_id = '{values[0]}' AND ic.index_id = '{values[1]}';";
        }

        /// <summary>
        /// Gets the query that finds all columns ina table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllColumnsInaTable(string schemaName, string tableName)
        {
            return $"SELECT [COLUMN_NAME] AS [Name], '' AS [Comment], CASE WHEN [IS_NULLABLE] = 'YES' THEN 1 ELSE 0 END AS [Nullable], [DATA_TYPE] AS [Type], CASE WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL AND [NUMERIC_PRECISION] IS NULL THEN '0' WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL THEN [NUMERIC_PRECISION] ELSE [CHARACTER_MAXIMUM_LENGTH] END AS [Scale], CASE WHEN [NUMERIC_SCALE] IS NULL THEN 0 ELSE [NUMERIC_SCALE] END AS [Precision], [COLUMN_DEFAULT] AS [Default], COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS [Auto] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = '{tableName}';";
        }

        /// <summary>
        /// Gets the query that finds all foreign keys ina table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllForeignKeysInaTable(string schemaName, string tableName)
        {
            return $"SELECT fk.name AS [Name], c.name AS [Column], rt.name AS [{ColumnName.ForeignTable}], rc.name AS [Reference_Column], fk.update_referential_action_desc AS [On_Update], fk.delete_referential_action_desc AS [On_Delete], '' AS [On_Match] FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id JOIN sys.tables t ON t.object_id = fkc.parent_object_id JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id JOIN sys.tables rt ON rt.object_id = fkc.referenced_object_id JOIN sys.columns rc ON rc.column_id = fkc.referenced_column_id AND rc.object_id = fkc.referenced_object_id WHERE t.name = '{tableName}' ;";
        }

        /// <summary>
        /// Gets the query that finds all indexes ina table.
        /// </summary>
        /// <param name="schemaName">Name of the schema.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns>A query.</returns>
        protected override string GetQueryThatFindsAllIndexesInaTable(string schemaName, string tableName)
        {
            return $"SELECT CASE  WHEN i.is_primary_key = 1 THEN 'primaryKey'  WHEN i.is_primary_key = 0 THEN 'index' END AS [Type], i.name AS [Name], i.is_unique AS [Unique], CONCAT(i.object_id, ':', i.index_id) AS [Id] FROM sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id WHERE t.name = '{tableName}';";
        }
    }
}