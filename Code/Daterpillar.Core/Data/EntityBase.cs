﻿using Ackara.Daterpillar.Annotation;
using System.Collections.Generic;
using System.Reflection;

namespace Ackara.Daterpillar.Data
{
    [System.Runtime.Serialization.DataContract]
    [System.Diagnostics.DebuggerDisplay("{" + nameof(ToDebuggerDisplay) + "}")]
    public abstract class EntityBase
    {
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName))
                {
                    TableAttribute attribute = GetType().GetTypeInfo().GetCustomAttribute<TableAttribute>();
                    _tableName = attribute?.Name;
                }

                return _tableName;
            }
        }

        public IEnumerable<ColumnInfo> GetKeys()
        {
            foreach (var column in GetColumns())
                if (column.IsKey)
                {
                    yield return column;
                }
        }

        public IEnumerable<ColumnInfo> GetColumns()
        {
            foreach (var property in GetType().GetRuntimeProperties())
            {
                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();

                if (column != null)
                {
                    object data = property.GetValue(this);

                    yield return new ColumnInfo()
                    {
                        AutoIncremented = column.AutoIncremented,
                        IsKey = column.IsKey,
                        Name = column.Name,
                        Value = data
                    };
                }
            }
        }

        protected virtual string ToDebuggerDisplay()
        {
            return $"[{TableName}]";
        }

        #region Private Members

        private string _tableName;

        #endregion Private Members
    }
}