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