using System;
using System.Collections.Generic;
using System.Reflection;

namespace Acklann.Daterpillar.Modeling
{
    internal static class ColumnMap
    {
        public static void Register(Type recordType)
        {
            string tableName = recordType.GetCustomAttribute<Attributes.TableAttribute>()?.Name;
            if (string.IsNullOrEmpty(tableName)) { System.Diagnostics.Debug.WriteLine($"'{recordType.FullName}' do not have a {nameof(Attributes.TableAttribute)}"); return; }

            if (_map.ContainsKey(tableName)) return;
            else _map.Add(tableName, null);

            foreach (PropertyInfo prop in recordType.GetColumns())
            {
                string qualifiedName = GetFullName(tableName, prop.Name);
                if (_map.ContainsKey(qualifiedName) == false)
                {
                    _map.Add(qualifiedName, prop);
                }
            }
        }

        public static PropertyInfo GetMember(string columnName)
        {
            return _map[columnName];
        }

        public static PropertyInfo GetMember(string tableName, string columnName)
        {
            return _map[GetFullName(tableName, columnName)];
        }

        public static string GetFullName(string tableName, string columnName) => string.Concat(tableName, '.', columnName);

        private static readonly IDictionary<string, PropertyInfo> _map = new Dictionary<string, PropertyInfo>();
    }
}