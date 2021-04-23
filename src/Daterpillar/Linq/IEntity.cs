using System;

namespace Acklann.Daterpillar.Linq
{
    

    [Obsolete]
    public interface IEntity
    {
        string GetTableName();

        string[] GetColumnList();

        object[] GetValueList();

        void Load(System.Data.IDataRecord record);
    }
}