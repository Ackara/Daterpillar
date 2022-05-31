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

            return result;
        }

        public void Add(string key, SqlValueArrayWriting plugin)
        {
            _create.Add(new KeyValuePair<string, SqlValueArrayWriting>(key, plugin));
        }

        public void OverrideSqlValueArrayItem<TRecord, TColumn>(string propertyName, Action<CreateOperationContext, TColumn> plugin)
            => Add(DefaultCrudOperations.CreateKey(typeof(TRecord).FullName, propertyName), new SqlValueArrayWriting((a, b) => { plugin.Invoke(a, (TColumn)b); }));

        public void OverrideSqlValueArrayItem<TRecord, TColumn>(Expression<Func<TRecord, object>> propertySelector, Action<CreateOperationContext, TColumn> plugin)
        {
            var expression = (MemberExpression)propertySelector.Body;
            Add(DefaultCrudOperations.CreateKey(typeof(TRecord).FullName, expression.Member.Name), new SqlValueArrayWriting((a, b) => { plugin.Invoke(a, (TColumn)b); }));
        }

        public void RegisterReadPlugin(string key, AfterSqlDataRecordLoaded plugin)
        {
            throw new NotImplementedException();
        }

        public void RegisterReadPlugin<TRecord>(Action<TRecord, IDataRecord> plugin)
        {
            throw new NotImplementedException();
        }

        #region Backing Members

        private readonly ICollection<KeyValuePair<string, SqlValueArrayWriting>> _create = new List<KeyValuePair<string, SqlValueArrayWriting>>();
        private readonly ICollection<ICrudOperations> _operations = new List<ICrudOperations>();

        #endregion Backing Members
    }
}