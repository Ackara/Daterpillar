using System;
using System.Collections.Generic;
using System.Text;

namespace Ackara.Daterpillar.Scripting
{
    public class SQLiteScriptBuilder : IScriptBuilder
    {
        public int Length => throw new NotImplementedException();

        public bool IsEmpty => throw new NotImplementedException();

        public string Append(string text)
        {
            throw new NotImplementedException();
        }

        public string Append(Schema schema)
        {
            throw new NotImplementedException();
        }

        public string Append(Table table)
        {
            throw new NotImplementedException();
        }

        public string Append(Column column)
        {
            throw new NotImplementedException();
        }

        public string Append(Index index)
        {
            throw new NotImplementedException();
        }

        public string Append(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public string AppendLine(string text)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            throw new NotImplementedException();
        }

        public string Remove(Schema schema)
        {
            throw new NotImplementedException();
        }

        public string Remove(Table table)
        {
            throw new NotImplementedException();
        }

        public string Remove(Column column)
        {
            throw new NotImplementedException();
        }

        public string Remove(Index index)
        {
            throw new NotImplementedException();
        }

        public string Remove(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public string UpdateColumn(Column oldColumn, Column newColumn)
        {
            throw new NotImplementedException();
        }

        public string UpdateTable(Table oldTable, Table newTable)
        {
            throw new NotImplementedException();
        }
    }
}
