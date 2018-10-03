using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Acklann.Daterpillar.Compilation
{
    public static class SchemaConvert
    {
        public static Schema ToSchema(string assemblyPath)
        {
            if (File.Exists(assemblyPath) == false) throw new FileNotFoundException($"Could not file assembly file at '{assemblyPath}'.", assemblyPath);

            Assembly assembly = System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            return ToSchema(assembly);
        }

        public static Schema ToSchema(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            IEnumerable<Type> tables = (from t in assembly.ExportedTypes
                                        where t.IsInterface == false && t.IsAbstract == false
                                        select t);

            var schema = new Schema();
            string documentation = Path.ChangeExtension(assembly.Location, ".xml");
            foreach (Type type in tables)
                try
                {
                    if (type.IsDefined(typeof(TableAttribute)))
                        schema.Add(ToTable(type, documentation));
                }
                catch (FileNotFoundException) { }

            return schema;
        }

        public static Table ToTable(Type type, string documentionPath = null)
        {
            var table = new Table();
            table.Name = type.GetCustomAttribute<TableAttribute>().Name;
            if (string.IsNullOrEmpty(table.Name))
            {
                int genericTypeDelimeter = type.Name.IndexOf('`');
                table.Name = (genericTypeDelimeter > 0 ? type.Name.Substring(genericTypeDelimeter) : type.Name);
            }

            IEnumerable<MemberInfo> members = type.GetMembers().Where(m => m.IsDefined(typeof(ColumnAttribute)));
            foreach (MemberInfo member in members)
            {
                ExtractColumnInfo(table, member, documentionPath);
            }
            ExtractIndexInfo(table, members);

            return table;
        }

        // Helper Methods
        // ============================================================

        private static void ExtractColumnInfo(Table table, MemberInfo member, string documentionPath)
        {
            var column = new Column();
            table.Columns.Add(column);
            ColumnAttribute columnAttr = member.GetCustomAttribute<ColumnAttribute>();
            column.Name = (string.IsNullOrEmpty(columnAttr.Name) ? member.Name : columnAttr.Name);
            column.AutoIncrement = columnAttr.AutoIncrement;
            column.DefaultValue = columnAttr.DefaultValue;
            column.IsNullable = columnAttr.Nullable;
            column.Table = table;

            string typeName = columnAttr.TypeName;
            if (string.IsNullOrEmpty(typeName))
            {
                if (member is PropertyInfo prop)
                {
                    typeName = CSharpTypeResolver.GetDataType(prop.PropertyType).Name;
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null) column.IsNullable = true;
                }
                else if (member is FieldInfo field)
                {
                    typeName = CSharpTypeResolver.GetDataType(field.FieldType).Name;
                    if (Nullable.GetUnderlyingType(field.FieldType) != null) column.IsNullable = true;
                }
            }

            DataTypeAttribute typeAttr = member.GetCustomAttribute(typeof(DataTypeAttribute)) as DataTypeAttribute;
            column.DataType = (typeAttr == null ? new DataType(typeName, columnAttr.Scale, columnAttr.Precision) : typeAttr.ToDataType());

            ExtractForiegnKeyInfo(table, member, column.Name);
        }

        private static void ExtractForiegnKeyInfo(Table table, MemberInfo member, string columnName)
        {
            if (member.GetCustomAttribute(typeof(ForeignKeyAttribute)) is ForeignKeyAttribute fk)
            {
                string foreignTable = fk.ForeignTable, foreignColumn = fk.ForeignColumn;

                var fTableType = Type.GetType(fk.ForeignTable);
                if (fTableType != null)
                {
                    TableAttribute ta = fTableType.GetCustomAttribute<TableAttribute>();
                    foreignTable = (string.IsNullOrEmpty(ta.Name) ? fTableType.Name : ta.Name);

                    MemberInfo fColumn = fTableType.GetMember(fk.ForeignColumn).FirstOrDefault(x => x.IsDefined(typeof(ColumnAttribute)));
                    if (fColumn?.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute ca)
                    {
                        foreignColumn = (string.IsNullOrEmpty(ca.Name) ? fColumn.Name : ca.Name);
                    }
                }

                table.ForeignKeys.Add(new ForeignKey()
                {
                    Table = table,
                    OnDelete = fk.OnDelete,
                    OnUpdate = fk.OnUpdate,
                    LocalColumn = columnName,
                    ForeignTable = foreignTable,
                    ForeignColumn = foreignColumn
                });
            }
        }

        private static void ExtractIndexInfo(Table table, IEnumerable<MemberInfo> members)
        {
            var indecies = new List<(string, IndexAttribute)>();
            foreach (MemberInfo member in members)
                if (member.GetCustomAttribute(typeof(ColumnAttribute)) is ColumnAttribute col)
                {
                    string columnName = (string.IsNullOrEmpty(col.Name) ? member.Name : col.Name);

                    if (member.GetCustomAttribute(typeof(IndexAttribute)) is IndexAttribute idx)
                    {
                        indecies.Add((columnName, idx));
                    }
                    else if (member.GetCustomAttribute(typeof(KeyAttribute)) is KeyAttribute key)
                    {
                        indecies.Add((columnName, new IndexAttribute(nameof(IndexType.PrimaryKey), IndexType.PrimaryKey) { Order = key.Order }));
                    }
                    else if (member.IsDefined(typeof(ForeignKeyAttribute)))
                    {
                        indecies.Add((columnName, new IndexAttribute(IndexType.Index)));
                    }
                }

            foreach (var index in indecies.GroupBy(a => a.Item2.Name))
            {
                Index idx = null;
                var names = new List<ColumnName>();
                foreach ((string columnName, IndexAttribute attr) in index)
                {
                    if (idx == null)
                    {
                        idx = new Index(attr.Type, attr.Unique);
                    }

                    names.Add(new ColumnName(columnName, attr.Order));
                }
                idx.Columns = names.ToArray();
                table.Indexes.Add(idx);
                idx.Table = table;
            }
        }
    }
}