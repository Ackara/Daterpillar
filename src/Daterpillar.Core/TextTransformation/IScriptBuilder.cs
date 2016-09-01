namespace Gigobyte.Daterpillar.TextTransformation
{
    public interface IScriptBuilder
    {
        void Create(Table table);

        void Create(Index index);

        void Drop(Table table);

        void Drop(Index index);

        void AlterTable(Table tableA, Table tableB);
    }
}