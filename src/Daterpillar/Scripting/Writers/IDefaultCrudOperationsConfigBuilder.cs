using System;
using System.Data;
using System.Linq.Expressions;

namespace Acklann.Daterpillar.Scripting.Writers
{
    public interface IDefaultCrudOperationsConfigBuilder
    {
        void OverrideSqlValueArrayItem(string key, SqlValueArrayBuilding plugin);

        void OverrideSqlValueArrayItem<TTable>(string memberName, Action<SqlValueArrayPluginContext, TTable> plugin);

        void OverrideSqlValueArrayItem<TTable>(Expression<Func<TTable, object>> memberSelector, Action<SqlValueArrayPluginContext, TTable> plugin);

        void OnAfterSqlDataRecordLoaded(string key, AfterSqlDataRecordLoaded plugin);

        void OnAfterSqlDataRecordLoaded<TRecord>(Action<TRecord, IDataRecord> plugin);

        void Add(ICrudBuilder operations);

        ICrudBuilder Build();
    }
}