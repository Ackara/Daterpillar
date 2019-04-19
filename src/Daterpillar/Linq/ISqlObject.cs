namespace Acklann.Daterpillar.Linq
{
    public interface ISqlObject
    {
        string TableName { get; }

        string[] GetColumnList();

        object[] GetValueList();
    }
}