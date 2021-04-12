using Acklann.Daterpillar.Attributes;
using Acklann.Daterpillar.Configuration;
using Acklann.Daterpillar.Serialization;
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

            IEnumerable<Type> tables = (from t in assembly.DefinedTypes
                                        where t.IsInterface == false && t.IsAbstract == false && t.IsDefined(typeof(TableAttribute))
                                        select t);

            var schema = new Schema { Version = assembly.GetName().Version.ToString(3) };
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
            IEnumerable<MemberInfo> columnCandidates = (from m in type.GetRuntimeProperties().Cast<MemberInfo>().Concat(type.GetRuntimeFields())
                                                        where
                                                         m.IsDefined(typeof(SqlIgnoreAttribute)) == false
                                                        select m).ToArray();

            var table = new Table() { Id = type.GetId() };
            SetTableInfo(table, type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>());
            SetTableInfo(table, type.GetCustomAttribute<TableAttribute>());
            SetDefaults(table, type);

            foreach (MemberInfo member in columnCandidates)
            {
                bool not_opt_in = (member.IsDefined(typeof(ColumnAttribute)) == false && member.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) == false);

                switch (member)
                {
                    case PropertyInfo prop:
                        if (not_opt_in && (prop.CanWrite == false)) continue;
                        break;

                    case FieldInfo field:
                        if (not_opt_in) continue;
                        break;
                }

                GetColumnInfo(table, member);
            }
            ExtractIndexInfo(table, columnCandidates);

            return table;
        }

        #region Backing Members

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

        // ==================== Table Information ==================== //

        private static void SetTableInfo(Table table, System.ComponentModel.DataAnnotations.Schema.TableAttribute attribute)
        {
            if (attribute == null) return;

            if (!string.IsNullOrEmpty(attribute.Name))
                table.Name = attribute.Name;
        }

        private static void SetTableInfo(Table table, TableAttribute attribute)
        {
            if (attribute == null) return;

            if (!string.IsNullOrEmpty(attribute.Name))
                table.Name = attribute.Name;
        }

        private static void SetDefaults(Table table, Type type)
        {
            if (string.IsNullOrEmpty(table.Name))
                table.Name = type.Name;
        }

        // ==================== Column Information ==================== //
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

            var dataType = new DataType(columnAttr);
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

            var stringLenAttr = (dataType.Scale == default ? member.GetCustomAttribute<System.ComponentModel.DataAnnotations.StringLengthAttribute>() : null);
            if (stringLenAttr != null) dataType.Scale = stringLenAttr.MaximumLength;

            var maxLenAttr = (dataType.Scale == default ? member.GetCustomAttribute<System.ComponentModel.DataAnnotations.MaxLengthAttribute>() : null);
            if (maxLenAttr != null) dataType.Scale = maxLenAttr.Length;

            DataTypeAttribute typeAttr = member.GetCustomAttribute(typeof(DataTypeAttribute)) as DataTypeAttribute;
            column.DataType = (typeAttr == null ? dataType : typeAttr.ToDataType());

            ExtractForiegnKeyInfo(table, member, column.GetIdOrName());
        }

        private static void GetColumnInfo(Table table, MemberInfo member)
        {
            var column = new Column();
            table.Add(column);

            SetColumnInfo(column, member.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.ColumnAttribute>());
            SetColumnInfo(column, member.GetCustomAttribute<System.ComponentModel.DataAnnotations.DataTypeAttribute>(), member);
            SetColumnInfo(column, member.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>());
            SetColumnInfo(column, member.GetCustomAttribute<ColumnAttribute>(), member);
            SetColumnDefaults(column, member);
        }

        private static void SetColumnInfo(Column column, System.ComponentModel.DataAnnotations.Schema.ColumnAttribute attribute)
        {
            if (attribute == null) return;
            column.Name = string.IsNullOrEmpty(attribute.Name) ? column.Name : attribute.Name;
        }

        private static void SetColumnInfo(Column column, System.ComponentModel.DataAnnotations.DataTypeAttribute attribute, MemberInfo member)
        {
            if (attribute == null) return;

            int scale = member != default ? member.GetMaxLength() : 0;

            switch (attribute.DataType)
            {
                case System.ComponentModel.DataAnnotations.DataType.CreditCard:
                case System.ComponentModel.DataAnnotations.DataType.Currency:
                case System.ComponentModel.DataAnnotations.DataType.Custom:
                case System.ComponentModel.DataAnnotations.DataType.EmailAddress:
                case System.ComponentModel.DataAnnotations.DataType.ImageUrl:
                case System.ComponentModel.DataAnnotations.DataType.Password:
                case System.ComponentModel.DataAnnotations.DataType.PhoneNumber:
                case System.ComponentModel.DataAnnotations.DataType.PostalCode:
                case System.ComponentModel.DataAnnotations.DataType.Url:
                case System.ComponentModel.DataAnnotations.DataType.Upload:
                    column.DataType = new DataType(SchemaType.VARCHAR, scale);
                    break;

                case System.ComponentModel.DataAnnotations.DataType.Date:
                    column.DataType = new DataType(SchemaType.DATE, scale);
                    break;

                case System.ComponentModel.DataAnnotations.DataType.DateTime:
                    column.DataType = new DataType(SchemaType.DATETIME, scale);
                    break;

                case System.ComponentModel.DataAnnotations.DataType.Time:
                case System.ComponentModel.DataAnnotations.DataType.Duration:
                    column.DataType = new DataType(SchemaType.BIGINT, scale);
                    break;

                case System.ComponentModel.DataAnnotations.DataType.Text:
                case System.ComponentModel.DataAnnotations.DataType.Html:
                case System.ComponentModel.DataAnnotations.DataType.MultilineText:
                    column.DataType = new DataType(SchemaType.TEXT, scale);
                    break;
            }
        }

        private static void SetColumnInfo(Column column, System.ComponentModel.DefaultValueAttribute attribute)
        {
            if (attribute == null) return;
            if (attribute.Value != null) column.DefaultValue = Convert.ToString(attribute.Value);
        }

        private static void SetColumnInfo(Column column, ColumnAttribute attribute, MemberInfo member)
        {
            if (attribute == null) return;

            if (!string.IsNullOrEmpty(attribute.Name))
                column.Name = attribute.Name;

            if (attribute.DefaultValue != null)
                column.DefaultValue = Convert.ToString(attribute.DefaultValue);

            if (!string.IsNullOrEmpty(attribute.TypeName))
                column.DataType = new DataType(attribute, member.GetMaxLength());

            column.IsNullable = attribute.Nullable;
            column.AutoIncrement = attribute.AutoIncrement;
        }

        private static void SetColumnDefaults(Column column, MemberInfo member)
        {
            if (string.IsNullOrEmpty(column.Name))
                column.Name = member.Name;

            if (string.IsNullOrEmpty(column.DataType.Name))
            {
                if (member is PropertyInfo prop)
                {
                    column.DataType = CSharpTranslator.GetDataType(prop.PropertyType);
                    if (Nullable.GetUnderlyingType(prop.PropertyType) != null) column.IsNullable = true;
                }
                else if (member is FieldInfo field)
                {
                    column.DataType = CSharpTranslator.GetDataType(field.FieldType);
                    if (Nullable.GetUnderlyingType(field.FieldType) != null) column.IsNullable = true;
                }
            }
        }

        // ==================== Foreign Key Information ==================== //

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

            foreach (var index in indecies.GroupBy(a => (string.IsNullOrEmpty(a.Item2.Name) ? a.Item1 : a.Item2.Name)))
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

        #endregion Backing Members
    }
}