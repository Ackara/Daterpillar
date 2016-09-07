using System;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class MSSQLScriptBuilder : IScriptBuilder
    {
        public void AlterTable(Column oldColumn, Column newColumn)
        {
            throw new NotImplementedException();
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            throw new NotImplementedException();
        }

        public void Append(string text)
        {
            throw new NotImplementedException();
        }

        public void AppendLine(string text)
        {
            throw new NotImplementedException();
        }

        public void Create(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public void Create(Index index)
        {
            throw new NotImplementedException();
        }

        public void Create(Column column)
        {
            throw new NotImplementedException();
        }

        public void Create(Table table)
        {
            throw new NotImplementedException();
        }

        public void Drop(Index index)
        {
            throw new NotImplementedException();
        }

        public void Drop(ForeignKey foreignKey)
        {
            throw new NotImplementedException();
        }

        public void Drop(Column column)
        {
            throw new NotImplementedException();
        }

        public void Drop(Table table)
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            throw new NotImplementedException();
        }
    }
}