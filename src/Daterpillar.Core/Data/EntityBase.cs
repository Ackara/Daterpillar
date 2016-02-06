using Gigobyte.Daterpillar.Annotation;
using System.Collections.Generic;
using System.Reflection;

namespace Gigobyte.Daterpillar.Data
{
    /// <summary>
    /// Represents an object that is mapped to a database table.
    /// </summary>
    [System.Runtime.Serialization.DataContract]
#if DEBUG
    [System.Diagnostics.DebuggerDisplay("{" + nameof(ToDebuggerDisplay) + "}")]
#endif
    public abstract class EntityBase
    {
        /// <summary>
        /// Gets the name of the database table that is mapped to this object.
        /// </summary>
        /// <value>The name of the table.</value>
        public string TableName
        {
            get
            {
                if (string.IsNullOrEmpty(_tableName))
                {
                    TableAttribute attribute = GetType().GetTypeInfo().GetCustomAttribute<TableAttribute>();
                    _tableName = attribute.Name;
                }

                return _tableName;
            }
        }

        /// <summary>
        /// Gets the primary key columns that are mapped to this object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ColumnInfo> GetKeys()
        {
            foreach (var column in GetColumns())
                if (column.IsKey)
                {
                    yield return column;
                }
        }

        /// <summary>
        /// Gets the columns that are mapped to this object.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ColumnInfo> GetColumns()
        {
            foreach (var property in GetType().GetRuntimeProperties())
            {
                ColumnAttribute column = property.GetCustomAttribute<ColumnAttribute>();

                if (column != null)
                {
                    object data = property.GetValue(this);

                    yield return new ColumnInfo(property)
                    {
                        AutoIncremented = column.AutoIncrement,
                        IsKey = column.IsKey,
                        Name = column.Name,
                        Value = data
                    };
                }
            }
        }

        /// <summary>
        /// Get this object string representation in the debugger variable windows.
        /// </summary>
        /// <returns>The string representation in the debugger variable windows.</returns>
        protected virtual string ToDebuggerDisplay()
        {
            return $"[{TableName}]";
        }

        #region Private Members

        private string _tableName;

        #endregion Private Members
    }
}