using System;
using System.Data;
using System.Linq.Expressions;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface IDefaultCrudOperationsConfigBuilder
    {
        void Add(string key, SqlValueArrayWriting plugin);

        void OverrideSqlValueArrayItem<TTable, TColumn>(string propertyName, Action<CreateOperationContext, TColumn> plugin);

        void OverrideSqlValueArrayItem<TTable, TColumn>(Expression<Func<TTable, object>> propertySelector, Action<CreateOperationContext, TColumn> plugin);

        void RegisterReadPlugin(string key, AfterSqlDataRecordLoaded plugin);

        void RegisterReadPlugin<TRecord>(Action<TRecord, IDataRecord> plugin);

        ICrudOperations Build();
    }
}