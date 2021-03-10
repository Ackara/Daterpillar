using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Acklann.Daterpillar.Modeling
{
    public abstract class DataViewRecord : IReadable
    {
        public DataViewRecord()
        {
            ColumnMap.Register(GetType());
        }

        public void Load(IDataRecord row)
        {
            int n = row.FieldCount;
            for (int i = 0; i < n; i++)
            {
                Read(row.GetName(i));
            }


            throw new NotImplementedException();
        }

        protected virtual void Read(string columnName)
        {

        }



        #region Backing Members

        

        #endregion
    }
}