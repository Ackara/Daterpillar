using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Translators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Migration
{
    public static class SchemaFactory
    {
        public static Schema CreateFrom(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            IEnumerable<Type> tables = (from t in assembly.ExportedTypes
                                        where t.IsInterface == false && t.IsAbstract == false && t.IsDefined(typeof(TableAttribute))
                                        select t);

            var v = assembly.GetName().Version;
            var schema = new Schema { Version = $"{v.Major}.{v.Minor}.{v.Build}" };
            string assemblyDocumentationFilePath = Path.ChangeExtension(assembly.Location, ".xml");

            foreach (Type type in tables)
            {
                try
                {
                    if (type.IsEnum)
                        ExtractEmunInfo(schema, type);
                    else
                        schema.Add(CreateFrom(type));
                }
                catch (FileNotFoundException) { System.Diagnostics.Debug.WriteLine($"Could not find {type.FullName} in the loaded assembly."); }
            }

            schema.Imports = new List<string>();
            foreach (IncludeAttribute attribute in assembly.GetCustomAttributes(typeof(IncludeAttribute)))
                schema.Imports.Add(attribute.FilePath);

            return schema;
        }

        public static Schema CreateFrom(string assemblyFilePath)
        {
            if (File.Exists(assemblyFilePath) == false) throw new FileNotFoundException($"Could not find assembly at '{assemblyFilePath}'.", assemblyFilePath);
            return CreateFrom(Assembly.Load(File.ReadAllBytes(assemblyFilePath)));
        }

        public static Table CreateFrom(Type type)
        {
            IEnumerable<MemberInfo> members = (from m in type.GetMembers()
                                               where
                                                (m.MemberType == MemberTypes.Property || m.MemberType == MemberTypes.Field)
                                                &&
                                                m.IsDefined(typeof(SqlIgnoreAttribute)) == false
                                               select m).ToArray();

            var table = new Table(type.GetName()) { Id = type.GetId() };
            foreach (MemberInfo member in members)
            {
                ExtractColumnInfo(table, member);
            }
            ExtractIndexInfo(table, members);

            return table;
        }

        // ==================== HELPERS ==================== //

        private static void ExtractEmunInfo(Schema schema, Type type)
        {
            var table = new Table(type.GetName(),
                new Column("Id", new DataType(SchemaType.INT)),
                new Column("Name", new DataType(SchemaType.VARCHAR)),

                new Index(IndexType.PrimaryKey, new ColumnName("Id")),
                new Index(IndexType.Index, true, new ColumnName("Name"))
                )
            { Id = type.GetId() };
            schema.Add(table);

            var script = new Script() { Name = $"{table.Name} seed-data" };
            var values = new StringBuilder();
            values.AppendLine($"INSERT INTO {table.GetIdOrName()} (Id, Name) VALUES ");
            foreach (MemberInfo member in type.GetMembers().Where(m => m.MemberType == MemberTypes.Field))
            {
                try
                {
                    object val = Enum.Parse(type, member.Name);
                    if (val != null)
                    {
                        values.AppendLine($"('{(int)val}', '{member.GetEnumName()}'),");
                    }
                }
                catch (ArgumentException) { }
            }
            script.Content = string.Concat(values.ToString().Trim().TrimEnd(' ', ','), ';');
            schema.Add(script);
        }

        private static void ExtractColumnInfo(Table table, MemberInfo member)
        {
            var column = new Column();
            table.Add(column);

            var columnAttr = member.GetCustomAttribute(typeof(ColumnAttribute)) as ColumnAttribute;
            var defaultAttr = member.GetCustomAttribute(typeof(DefaultValueAttribute)) as DefaultValueAttribute;

            column.DefaultValue = Convert.ToString(columnAttr?.DefaultValue ?? defaultAttr?.Value);
            if (string.IsNullOrEmpty(column.DefaultValue)) column.DefaultValue = null;
            column.AutoIncrement = (columnAttr?.AutoIncrement ?? false);
            column.IsNullable = (columnAttr?.Nullable ?? false);
            column.Name = member.GetName();
            column.Id = member.GetId();

            var dataType = new DataType(columnAttr?.TypeName);

            if (string.IsNullOrEmpty(dataType.Name))
            {
                if (member is PropertyInfo prop)
                {
                    dataType = CSharpTranslator.GetDataType(prop.PropertyType);
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null) column.IsNullable = true;
                }
                else if (member is FieldInfo field)
                {
                    dataType = CSharpTranslator.GetDataType(field.FieldType);
                    if (Nullable.GetUnderlyingType(field.FieldType) != null) column.IsNullable = true;
                }
            }

            DataTypeAttribute typeAttr = member.GetCustomAttribute(typeof(DataTypeAttribute)) as DataTypeAttribute;
            column.DataType = (typeAttr == null ? dataType : typeAttr.ToDataType());

            ExtractForiegnKeyInfo(table, member, column.GetIdOrName());
        }

        private static void ExtractForiegnKeyInfo(Table table, MemberInfo member, string columnName)
        {
            if (member.GetCustomAttribute(typeof(ForeignKeyAttribute)) is ForeignKeyAttribute fkAttr)
            {
                string foreignTable = fkAttr.ForeignTable;
                string foreignColumn = fkAttr.ForeignColumn;

                Type referencedType = Type.GetType(fkAttr.ForeignTable);
                if (referencedType != null)
                {
                    foreignTable = referencedType.GetIdOrName();

                    MemberInfo referencedField = referencedType.GetMember(fkAttr.ForeignColumn).FirstOrDefault();
                    if (referencedField != null)
                    {
                        foreignColumn = referencedField.GetIdOrName();
                    }
                }

                table.ForeignKeys.Add(new ForeignKey()
                {
                    Table = table,
                    LocalColumn = columnName,
                    OnDelete = fkAttr.OnDelete,
                    OnUpdate = fkAttr.OnUpdate,
                    ForeignTable = foreignTable,
                    ForeignColumn = foreignColumn
                });
            }
        }

        private static void ExtractIndexInfo(Table table, IEnumerable<MemberInfo> members)
        {
            var indecies = new List<(string, IndexAttribute)>();
            foreach (MemberInfo member in members)
            {
                string columnName = member.GetIdOrName();

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