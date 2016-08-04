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
            throw new NotImplementedException();
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
