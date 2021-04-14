using Acklann.Daterpillar.Serialization;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    internal static class ColumnMap
    {
        public static void Register(Type recordType)
        {
            string tableName = recordType.GetCustomAttribute<Attributes.TableAttribute>()?.Name;
            if (string.IsNullOrEmpty(tableName)) return;

            if (_map.ContainsKey(tableName)) return;
            else _map.Add(tableName, null);

            PropertyInfo[] properties = recordType.GetColumns().ToArray();
            foreach (PropertyInfo prop in properties)
            {
                string qualifiedName = GetFullQualifiedName(tableName, prop.Name);
                if (_map.ContainsKey(qualifiedName) == false)
                {
                    _map.Add(qualifiedName, prop);
                }
            }

            _map.Add(GetFullQualifiedName(tableName, nameof(IInsertable.GetColumns)), (from prop in properties select prop.GetColumnName()).ToArray());
            _map.Add(GetFullQualifiedName(tableName, nameof(IInsertable.GetValues)), from prop in properties select properties);
        }

        public static PropertyInfo GetMember(string columnName)
        {
            return (PropertyInfo)_map[columnName];
        }

        public static PropertyInfo GetMember(string tableName, string columnName)
        {
            return (PropertyInfo)_map[GetFullQualifiedName(tableName, columnName)];
        }

        public static PropertyInfo[] GetProperties(string tableName)
        {
            return (PropertyInfo[])_map[GetFullQualifiedName(tableName, nameof(IInsertable.GetValues))];
        }

        public static string[] GetColumns(string tableName)
        {
            return (string[])_map[GetFullQualifiedName(tableName, nameof(IInsertable.GetColumns))];
        }

        public static string GetFullQualifiedName(string tableName, string columnName) => string.Concat(tableName, '.', columnName);

        private static readonly Hashtable _map = new Hashtable();
    }
}