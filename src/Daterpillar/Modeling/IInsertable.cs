namespace Acklann.Daterpillar.Modeling
{
    public interface IInsertable
    {
        string GetTableName();

        string[] GetColumns();

        object[] GetValues();
    }
}