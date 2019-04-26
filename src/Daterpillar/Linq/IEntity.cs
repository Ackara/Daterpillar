namespace Acklann.Daterpillar.Linq
{
    public interface IEntity
    {
        string GetTableName();

        string[] GetColumnList();

        object[] GetValueList();

        void Load(System.Data.IDataRecord record);
    }
}