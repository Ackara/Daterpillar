namespace Acklann.Daterpillar.Foo
{
    [System.Obsolete]
    public interface IInsertable
    {
        string GetTableName();

        string[] GetColumns();

        object[] GetValues();
    }
}