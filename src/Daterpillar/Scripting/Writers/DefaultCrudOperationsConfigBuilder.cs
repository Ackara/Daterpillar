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

        public void OverrideSqlValueArrayItem(string key, SqlValueArrayBuilding plugin)
        {
            _create.Add(new KeyValuePair<string, SqlValueArrayBuilding>(key, plugin));
        }

        public void OverrideSqlValueArrayItem<TTable>(string propertyName, Action<SqlValueArrayPluginContext, TTable> plugin)
        => OverrideSqlValueArrayItem(DefaultCrudOperations.CreateKey(typeof(TTable).FullName, propertyName), new SqlValueArrayBuilding((a, b) => { plugin.Invoke(a, (TTable)b); }));

        /// <summary>
        /// Overrides the SQL values array used for constructing INSERT statements.
        /// </summary>
        /// <typeparam name="TTable">The type that represent the table.</typeparam>
        /// <param name="propertySelector">The SQL column selector.</param>
        /// <param name="plugin">The method that overrides the values at the <paramref name="propertySelector"/> position.</param>
        public void OverrideSqlValueArrayItem<TTable>(Expression<Func<TTable, object>> propertySelector, Action<SqlValueArrayPluginContext, TTable> plugin)
        {
            string memberName = "";
            if (propertySelector.Body is MemberExpression me) memberName = me.Member.Name;
            else if (propertySelector.Body is UnaryExpression ue && ue.Operand is MemberExpression ueme) memberName = ueme.Member.Name;

            OverrideSqlValueArrayItem(DefaultCrudOperations.CreateKey(typeof(TTable).FullName, memberName), new SqlValueArrayBuilding((a, b) => { plugin.Invoke(a, (TTable)b); }));
        }

        public void OnAfterSqlDataRecordLoaded(string key, AfterSqlDataRecordLoaded plugin)
        {
            _read.Add(new KeyValuePair<string, AfterSqlDataRecordLoaded>(key, plugin));
        }

        public void OnAfterSqlDataRecordLoaded<TTable>(Action<TTable, IDataRecord> plugin)
        {
            _read.Add(new KeyValuePair<string, AfterSqlDataRecordLoaded>(typeof(TTable).FullName, new AfterSqlDataRecordLoaded((a, b) => { plugin.Invoke((TTable)a, b); })));
        }

        public void Add(ICrudOperations operations)
        {
            _operations.Add(operations);
        }

        internal IEnumerable<ICrudOperations> GetCrudOperations() => _operations;

        #region Backing Members

        private readonly ICollection<KeyValuePair<string, SqlValueArrayBuilding>> _create = new List<KeyValuePair<string, SqlValueArrayBuilding>>();
        private readonly ICollection<KeyValuePair<string, AfterSqlDataRecordLoaded>> _read = new List<KeyValuePair<string, AfterSqlDataRecordLoaded>>();
        private readonly ICollection<ICrudOperations> _operations = new List<ICrudOperations>();

        #endregion Backing Members
    }
}