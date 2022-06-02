using System;
using System.Data;
using System.Linq.Expressions;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface IDefaultCrudOperationsConfigBuilder
    {
        void OverrideSqlValueArrayItem(string key, SqlValueArrayWriting plugin);

        void OverrideSqlValueArrayItem<TTable, TColumn>(string propertyName, Action<SqlValueArrayPluginContext, TColumn> plugin);

        void OverrideSqlValueArrayItem<TTable, TColumn>(Expression<Func<TTable, object>> propertySelector, Action<SqlValueArrayPluginContext, TColumn> plugin);

        void OnAfterSqlDataRecordLoaded(string key, AfterSqlDataRecordLoaded plugin);

        void OnAfterSqlDataRecordLoaded<TRecord>(Action<TRecord, IDataRecord> plugin);

        ICrudOperations Build();
    }
}