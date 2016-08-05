using Gigobyte.Daterpillar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class MSSQLSchemaAggregator : SchemaAggregatorBase
    {
        public MSSQLSchemaAggregator(IDbConnection connection) : base(connection)
        {
        }

        protected override string GetColumnInfoQuery(string tableName)
        {
            return $"SELECT [COLUMN_NAME] AS [Name], '' AS [Comment], [IS_NULLABLE] AS [Nullable], [DATA_TYPE] AS [Type], CASE WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL AND [NUMERIC_PRECISION] IS NULL THEN '0' WHEN [CHARACTER_MAXIMUM_LENGTH] IS NULL THEN [NUMERIC_PRECISION] ELSE [CHARACTER_MAXIMUM_LENGTH] END AS [Scale], CASE WHEN [NUMERIC_SCALE] IS NULL THEN 0 ELSE [NUMERIC_SCALE] END AS [Precision], [COLUMN_DEFAULT] AS [Default], COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') AS [Auto] FROM [INFORMATION_SCHEMA].[COLUMNS] WHERE [TABLE_NAME] = '{tableName}';";
        }

        protected override string GetForeignKeyInfoQuery(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override string GetIndexInfoQuery(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override string GetTableInfoQuery()
        {
            return $"SELECT [TABLE_NAME] AS [Name], '' AS[Comment] FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';";
        }
    }
}
