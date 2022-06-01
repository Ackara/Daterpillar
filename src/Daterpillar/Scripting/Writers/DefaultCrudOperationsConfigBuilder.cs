using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Acklann.Daterpillar.Scripting.Writers
{
    internal class DefaultCrudOperationsConfigBuilder : IDefaultCrudOperationsConfigBuilder
    {
        public ICrudOperations Build()
        {
            var result = new DefaultCrudOperations();
            foreach (var plugin in _create) result.Add(plugin.Key, plugin.Value);
            foreach (var plugin in _read) result.Add(plugin.Key, plugin.Value);
            return result;
        }

        public void OverrideSqlValueArrayItem(string key, SqlValueArrayWriting plugin)
        {
            _create.Add(new KeyValuePair<string, SqlValueArrayWriting>(key, plugin));
        }

        public void OverrideSqlValueArrayItem<TRecord, TColumn>(string propertyName, Action<CreateOperationContext, TColumn> plugin)
            => OverrideSqlValueArrayItem(DefaultCrudOperations.CreateKey(typeof(TRecord).FullName, propertyName), new SqlValueArrayWriting((a, b) => { plugin.Invoke(a, (TColumn)b); }));

        public void OverrideSqlValueArrayItem<TRecord, TColumn>(Expression<Func<TRecord, object>> propertySelector, Action<CreateOperationContext, TColumn> plugin)
        {
            var expression = (MemberExpression)propertySelector.Body;
            OverrideSqlValueArrayItem(DefaultCrudOperations.CreateKey(typeof(TRecord).FullName, expression.Member.Name), new SqlValueArrayWriting((a, b) => { plugin.Invoke(a, (TColumn)b); }));
        }

        public void OnAfterSqlDataRecordLoaded(string key, AfterSqlDataRecordLoaded plugin)
        {
            _read.Add(new KeyValuePair<string, AfterSqlDataRecordLoaded>(key, plugin));
        }

        public void OnAfterSqlDataRecordLoaded<TRecord>(Action<TRecord, IDataRecord> plugin)
        {
            _read.Add(new KeyValuePair<string, AfterSqlDataRecordLoaded>(typeof(TRecord).FullName, new AfterSqlDataRecordLoaded((a, b) => { plugin.Invoke((TRecord)a, b); })));
        }

        #region Backing Members

        private readonly ICollection<KeyValuePair<string, SqlValueArrayWriting>> _create = new List<KeyValuePair<string, SqlValueArrayWriting>>();
        private readonly ICollection<KeyValuePair<string, AfterSqlDataRecordLoaded>> _read = new List<KeyValuePair<string, AfterSqlDataRecordLoaded>>();
        private readonly ICollection<ICrudOperations> _operations = new List<ICrudOperations>();

        #endregion Backing Members
    }
}