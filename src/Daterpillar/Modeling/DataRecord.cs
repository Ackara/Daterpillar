using System;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataRecord : DataViewRecord, IInsertable
    {
        public DataRecord()
        {
            ColumnMap.Register(GetType());
        }

        public string GetTableName()
        {
            throw new NotImplementedException();
        }

        public virtual string[] GetColumns()
        {
            

            throw new NotImplementedException();
        }

        public virtual object[] GetValues()
        {
            throw new NotImplementedException();
        }
    }
}