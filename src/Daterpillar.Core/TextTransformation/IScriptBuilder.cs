namespace Gigobyte.Daterpillar.TextTransformation
{
    public interface IScriptBuilder
    {
        void Append(string text);
        
        void AppendLine(string text);

        void Create(Table table);

        void Create(Index index);

        void Create(ForeignKey foreignKey);

        void Drop(Table table);

        void Drop(Index index);

        void Drop(ForeignKey foreignKey);

        void AlterTable(Table tableA, Table tableB);
                
        byte[] GetContentAsBytes();

        string GetContentAsString();
    }
}