using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class MSSQLSchemaAggregator : SchemaAggregatorBase
    {
        public MSSQLSchemaAggregator(string connectionString) : this(new System.Data.SqlClient.SqlConnection(connectionString))
        {
        }

        public MSSQLSchemaAggregator(IDbConnection connection) : base(connection)
        {
        }

        protected override string GetColumnInfoQuery(string tableName)
        {
            return $"SELECT [COLUMN_NAME] AS [Name], '' AS [Comment], [IS_NULLABLE] AS [Nullable], [DATA_TYPE] AS [Type], CASE WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL AND [NUMERIC_PRECISION] IS NULL THEN '0' WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL THEN [NUMERIC_PRECISION] ELSE [CHARACTER_MAXIMUM_LENGTH] END AS [Scale], CASE WHEN [NUMERIC_SCALE] IS NULL THEN 0 ELSE [NUMERIC_SCALE] END AS [Precision], [COLUMN_DEFAULT] AS [Default], COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS [Auto] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = '{tableName}';";
        }

        protected override string GetForeignKeyInfoQuery(string tableName)
        {
            return $"SELECT fk.name AS [Name], c.name AS [Column], rt.name AS [Referecne_Table], rc.name AS [Reference_Column], fk.update_referential_action_desc AS [On_Update], fk.delete_referential_action_desc AS [On_Delete], '' AS [On_Match] FROM sys.foreign_keys fk JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id JOIN sys.tables t ON t.object_id = fkc.parent_object_id JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id JOIN sys.tables rt ON rt.object_id = fkc.referenced_object_id JOIN sys.columns rc ON rc.column_id = fkc.referenced_column_id AND rc.object_id = fkc.referenced_object_id WHERE t.name = '{tableName}' ;";
        }

        protected override string GetIndexColumnsQuery(string indexIdentifier)
        {
            string[] values = indexIdentifier.Split(':');
            return $"SELECT c.name AS [Name], CASE  WHEN ic.is_descending_key = 1 THEN 'DESC'  ELSE 'ASC' END AS [Order] FROM sys.index_columns ic JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id WHERE ic.object_id = '{values[0]}' AND ic.index_id = '{values[1]}';";
        }

        protected override string GetIndexInfoQuery(string tableName)
        {
            return $"SELECT CASE  WHEN i.is_primary_key = 1 THEN 'primaryKey'  WHEN i.is_primary_key = 0 THEN 'index' END AS [Type], i.name AS [Name], i.is_unique_constraint AS [Unique], CONCAT(i.object_id, ':', i.index_id) AS [Id] FROM sys.indexes i JOIN sys.tables t ON t.object_id = i.object_id WHERE t.name = '{tableName}';";
        }

        protected override string GetTableInfoQuery()
        {
            return $"SELECT [TABLE_NAME] AS [Name], '' AS[Comment] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
        }
    }
}