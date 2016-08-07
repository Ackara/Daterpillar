using System;
using System.Data;

namespace Gigobyte.Daterpillar.Data
{
    public class SQLiteSchemaAggregator : SchemaAggregatorBase
    {
        public SQLiteSchemaAggregator(IDbConnection connection) : base(connection)
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
            throw new NotImplementedException();
        }
    }
}