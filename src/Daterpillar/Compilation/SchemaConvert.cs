using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Compilation
{
    public static class SchemaConvert
    {
        public static Schema ToSchema(string assemblyPath)
        {
            if (File.Exists(assemblyPath) == false) throw new FileNotFoundException($"Could not file assembly file at '{assemblyPath}'.", assemblyPath);

            return ToSchema(System.Runtime.Loader.AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath));
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
            {
                try
                {
                    if (type.IsDefined(typeof(TableAttribute)))
                    {
                        if (type.IsEnum)
                            ExtractEmunInfo(schema, type, documentation);
                        else
                            schema.Add(ToTable(type, documentation));
                    }
                }
                catch (FileNotFoundException) { System.Diagnostics.Debug.WriteLine("Could not find a .dll."); }
            }

            if (assembly.GetCustomAttribute(typeof(IncludeAttribute)) is IncludeAttribute attr)
                schema.Include = attr.Path;

            return schema;
        }

        public static Table ToTable(Type type, string documentionPath = null)
        {
            IEnumerable<MemberInfo> members = (from m in type.GetMembers()
                                               where
                                                (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                                                &&
                                                m.IsDefined(typeof(SqlIgnoreAttribute)) == false
                                               select m).ToArray();

            var table = new Table(GetName(type)) { Id = GetId(type) };
            foreach (MemberInfo member in members)
            {
                ExtractColumnInfo(table, member, documentionPath);
            }
            ExtractIndexInfo(table, members);

            return table;
        }

        // ==================== HELPER ==================== //

        internal static string GetName(MemberInfo member)
        {
            var columnAttr = member?.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var nameAttr = member?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(columnAttr?.Name) ? nameAttr?.DisplayName : columnAttr?.Name) ?? member.Name;
        }

        internal static string GetName(Type type)
        {
            var tableAttr = type?.GetCustomAttribute(typeof(TableAttribute)) as TableAttribute;
            var nameAttr = type?.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            string name = (string.IsNullOrEmpty(tableAttr?.Name) ? nameAttr?.DisplayName : tableAttr?.Name);

            if (string.IsNullOrEmpty(name))
            {
                int genericTypeDelimeter = type.Name.IndexOf('`');
                name = (genericTypeDelimeter > 0 ? type.Name.Substring(genericTypeDelimeter) : type.Name);
            }

            return name;
        }

        internal static string GetEnumName(MemberInfo member)
        {
            var enumAttr = member.GetCustomAttribute(typeof(EnumValueAttribute)) as EnumValueAttribute;
            var nameAttr = member.GetCustomAttribute(typeof(DisplayNameAttribute)) as DisplayNameAttribute;

            return (string.IsNullOrEmpty(enumAttr?.Name) ? nameAttr?.DisplayName : enumAttr?.Name) ?? member.Name;
        }

        internal static int GetId(MemberInfo member)
        {
            if (member.GetCustomAttribute(typeof(SUIDAttribute)) is SUIDAttribute attr)
            {
                return attr.Id;
            }

            return 0;
        }

        private static void ExtractEmunInfo(Schema schema, Type type, string documentation)
        {
            var table = new Table(GetName(type),
                new Column("Id", new DataType(SchemaType.INT)),
                new Column("Name", new DataType(SchemaType.VARCHAR)),

                new Index(IndexType.PrimaryKey, new ColumnName("Id")),
                new Index(IndexType.Index, true, new ColumnName("Name"))
                );
            schema.Add(table);

            var script = new Script() { Name = $"{table.Name} seed-data" };
            var values = new StringBuilder();
            values.AppendLine($"INSERT INTO {table.Name} (Id, Name) VALUES ");
            foreach (var member in type.GetMembers().Where(m => m.MemberType == MemberTypes.Field))
            {
                try
                {
                    object val = Enum.Parse(type, member.Name);
                    if (val != null)
                    {
                        System.Diagnostics.Debug.WriteLine((int)val);
                        System.Diagnostics.Debug.WriteLine(GetEnumName(member));
                        values.AppendLine($"('{(int)val}', '{GetEnumName(member)}'),");
                    }
                }
                catch (ArgumentException) { }
            }
            script.Content = string.Concat(values.ToString().Trim().TrimEnd(' ', ','), ';');
            schema.Add(script);
        }

        private static void ExtractColumnInfo(Table table, MemberInfo member, string documentionPath)
        {
            var column = new Column();
            table.Columns.Add(column);

            var columnAttr = member.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var defaultAttr = member.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;

            column.DefaultValue = Convert.ToString(columnAttr?.DefaultValue ?? defaultAttr?.Value);
            if (string.IsNullOrEmpty(column.DefaultValue)) column.DefaultValue = null;
            column.AutoIncrement = (columnAttr?.AutoIncrement ?? false);
            column.IsNullable = (columnAttr?.Nullable ?? false);
            column.Name = GetName(member);
            column.Id = GetId(member);
            column.Table = table;

            var dataType = new DataType(columnAttr?.TypeName);
            if (string.IsNullOrEmpty(dataType.Name))
            {
                if (member is PropertyInfo prop)
                {
                    dataType = CSharpTypeResolver.GetDataType(prop.PropertyType);
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null) column.IsNullable = true;
                }
                else if (member is FieldInfo field)
                {
                    dataType = CSharpTypeResolver.GetDataType(field.FieldType);
                    if (Nullable.GetUnderlyingType(field.FieldType) != null) column.IsNullable = true;
                }
            }

            DataTypeAttribute typeAttr = member.GetCustomAttribute(typeof(DataTypeAttribute)) as DataTypeAttribute;
            column.DataType = (typeAttr == null ? dataType : typeAttr.ToDataType());

            ExtractForiegnKeyInfo(table, member, column.Name);
        }

        private static void ExtractForiegnKeyInfo(Table table, MemberInfo member, string columnName)
        {
            if (member.GetCustomAttribute(typeof(ForeignKeyAttribute)) is ForeignKeyAttribute fkAttr)
            {
                string foreignTable = fkAttr.ForeignTable, foreignColumn = fkAttr.ForeignColumn;

                var fTableType = Type.GetType(fkAttr.ForeignTable);
                if (fTableType != null)
                {
                    foreignTable = GetName(fTableType);

                    MemberInfo fColumn = fTableType.GetMember(fkAttr.ForeignColumn).FirstOrDefault();
                    if (fColumn != null)
                        foreignColumn = GetName(fColumn);
                }

                table.ForeignKeys.Add(new ForeignKey()
                {
                    Table = table,
                    LocalColumn = columnName,
                    OnDelete = fkAttr.OnDelete,
                    OnUpdate = fkAttr.OnUpdate,
                    ForeignTable = foreignTable,
                    ForeignColumn = (foreignColumn)
                });
            }
        }

        private static void ExtractIndexInfo(Table table, IEnumerable<MemberInfo> members)
        {
            var indecies = new List<(string, IndexAttribute)>();
            foreach (MemberInfo member in members)
            {
                string columnName = GetName(member);

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
                table.Indecies.Add(idx);
                idx.Table = table;
            }
        }
    }
}