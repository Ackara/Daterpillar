namespace Acklann.Daterpillar.Foo
{
    public interface IInsertable
    {
        string GetTableName();

        string[] GetColumns();

        object[] GetValues();
    }
}