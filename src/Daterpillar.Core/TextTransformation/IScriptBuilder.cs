namespace Gigobyte.Daterpillar.TextTransformation
{
    public interface IScriptBuilder
    {
        string GetContent();

        void Append(string text);

        void AppendLine(string text);

        void Create(Schema schema);

        void Create(Table table);

        void Create(Column column);

        void Create(Index index);

        void Create(ForeignKey foreignKey);

        void Drop(Schema schema);

        void Drop(Table table);

        void Drop(Column column);

        void Drop(Index index);

        void Drop(ForeignKey foreignKey);

        void AlterTable(Table oldTable, Table newTable);

        void AlterTable(Column oldColumn, Column newColumn);
    }
}