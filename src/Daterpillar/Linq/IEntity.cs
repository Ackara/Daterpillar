namespace Acklann.Daterpillar.Linq
{
    public delegate IEntity EntityConstructor();

    public interface IEntity
    {
        string GetTableName();

        string[] GetColumnList();

        object[] GetValueList();

        EntityConstructor GetConstructor();

        void Load(System.Data.IDataRecord record);
    }
}