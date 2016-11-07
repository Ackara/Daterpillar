namespace Gigobyte.Daterpillar.TextTransformation
{
    public class NullScriptBuilder : IScriptBuilder
    {
        public void AlterTable(Column oldColumn, Column newColumn)
        {
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
        }

        public void Append(string text)
        {
        }

        public void AppendLine(string text)
        {
        }

        public void Clear()
        {
        }

        public void Create(Index index)
        {
        }

        public void Create(ForeignKey foreignKey)
        {
        }

        public void Create(Column column)
        {
        }

        public void Create(Table table)
        {
        }

        public void Create(Schema schema)
        {
        }

        public void Drop(Column column)
        {
        }

        public void Drop(ForeignKey foreignKey)
        {
        }

        public void Drop(Index index)
        {
        }

        public void Drop(Table table)
        {
        }

        public void Drop(Schema schema)
        {
        }

        public string GetContent()
        {
            return string.Empty;
        }
    }
}