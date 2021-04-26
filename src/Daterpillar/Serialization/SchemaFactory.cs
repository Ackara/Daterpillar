using Acklann.Daterpillar.Modeling.Attributes;
using Acklann.Daterpillar.Scripting.Translators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Acklann.Daterpillar.Serialization
{
    public static class SchemaFactory
    {
        public static Schema CreateFrom(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            IEnumerable<Type> tables = (from t in assembly.DefinedTypes
                                        where
                                            t.IsInterface == false &&
                                            t.IsAbstract == false &&
                                            (t.IsDefined(typeof(TableAttribute)) || t.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.TableAttribute)))
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
            IEnumerable<MemberInfo> columnCandidates =
                (from property in type.GetProperties()
                 let not_explictly_defined = property.IsDefined(typeof(ColumnAttribute)) == false && property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute)) == false
                 where
                    /*not ignored*/property.IsDefined(typeof(SqlIgnoreAttribute)) == false && property.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) == false
                    &&
                    (not_explictly_defined && property.CanWrite == false) == false
                 select (MemberInfo)property)

                 .Concat

                 (from field in type.GetRuntimeFields()
                  let isExplict = field.IsDefined(typeof(ColumnAttribute)) || field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.ColumnAttribute))
                  where
                     field.IsDefined(typeof(SqlIgnoreAttribute)) == false && field.IsDefined(typeof(System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute)) == false
                     &&
                     isExplict
                  select field);

            var table = new Table() { Id = type.GetId() };
            SetTableInfo(table, type.GetCustomAttribute<System.ComponentModel.DataAnnotations.Schema.TableAttribute>());
            SetTableInfo(table, type.GetCustomAttribute<TableAttribute>());
            SetDefaults(table, type);

            SetColumns(table, columnCandidates);
            SetIndecies(table, columnCandidates);
            SetForeignKeys(table, columnCandidates);

            return table;
        }

        #region Backing Members

        private static void ExtractEmunInfo(Schema schema, Type type)
        {
            var table = new Table(type.GetTableName(),
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
        private static void SetColumns(Table table, IEnumerable<MemberInfo> columnCandidates)
        {
            foreach (MemberInfo member in columnCandidates)
            {
                SetColumnInfo(table, member);
            }
        }

        private static void SetColumnInfo(Table table, MemberInfo member)
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

        // ==================== Index Information ==================== //

        private static void SetIndecies(Table table, IEnumerable<MemberInfo> members)
        {
            /// An index may include 2 or more columns. The index will have to be grouped by name or by primary key.
            /// One one primary key can exists on a table so therefore the index name is ignored.
            /// STEPS:
            /// 1. Caputre all indexes on all columns
            /// 2. Combine with index with the same name into one
            /// 3. Combine all the primary key index into one.

            // 1. Capture
            var candiates = new List<Index>();

            foreach (MemberInfo member in members)
            {
                Index index = null;

                index = SetIndexInfo(index, member.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>());
                index = SetIndexInfo(index, member.GetCustomAttribute<Acklann.Daterpillar.Modeling.Attributes.KeyAttribute>());
                index = SetIndexInfo(index, member.GetCustomAttribute<Acklann.Daterpillar.Modeling.Attributes.IndexAttribute>());
                SetIndexDefault(index, member, table);

                if (index != null) candiates.Add(index);
            }

            var results = new List<Index>();
            IList<ColumnName> columns;

            // 2. Combine by name.
            foreach (var group in candiates.Where(x => x.Type == IndexType.Index).GroupBy(x => x.Name))
            {
                Index anchor = group.FirstOrDefault();
                if (anchor == null) continue;

                columns = new List<ColumnName>();
                foreach (Index index in group) columns.Add(index.Columns[0]);
                table.Add(new Index(IndexType.Index, columns.ToArray()));
            }

            // 3. Combine primary-keys
            if (candiates.Any(x => x.Type == IndexType.PrimaryKey))
                table.Add(new Index(IndexType.PrimaryKey, (from x in candiates
                                                           where x.Type == IndexType.PrimaryKey
                                                           select x.Columns[0]).ToArray()));
        }

        private static Index SetIndexInfo(Index index, System.ComponentModel.DataAnnotations.KeyAttribute attribute)
        {
            if (attribute == null) return index;
            if (index == null) index = new Index();

            index.Type = IndexType.PrimaryKey;
            return index;
        }

        private static Index SetIndexInfo(Index index, KeyAttribute attribute)
        {
            if (attribute == null) return index;
            if (index == null) index = new Index();

            index.Type = IndexType.PrimaryKey;
            return index;
        }

        private static Index SetIndexInfo(Index index, IndexAttribute attribute)
        {
            if (attribute == null) return index;
            if (index == null) index = new Index();

            index.Name = attribute.Name;
            index.IsUnique = attribute.Unique;
            index.Type = attribute.Type;
            return index;
        }

        private static void SetIndexDefault(Index index, MemberInfo member, Table table)
        {
            if (index == null) return;
            index.Table = table;
            index.Columns = new ColumnName[] { member.GetColumnName() };
        }

        // ==================== Foreign Key Information ==================== //

        private static void SetForeignKeys(Table table, IEnumerable<MemberInfo> memebers)
        {
            foreach (var member in memebers)
            {
                if (member.GetCustomAttribute(typeof(ForeignKeyAttribute)) is ForeignKeyAttribute fkAttr)
                {
                    string foreignTable = fkAttr.ForeignTable;
                    string foreignColumn = fkAttr.ForeignColumn;

                    Type referencedType = Type.GetType(fkAttr.ForeignTable);
                    if (referencedType != null)
                    {
                        foreignTable = referencedType.GetTableName();

                        MemberInfo referencedField = referencedType.GetMember(fkAttr.ForeignColumn).FirstOrDefault();
                        if (referencedField != null)
                        {
                            foreignColumn = referencedField.GetColumnName();
                        }
                    }

                    table.Add(new ForeignKey()
                    {
                        Table = table,
                        LocalColumn = member.GetColumnName(),
                        OnDelete = fkAttr.OnDelete,
                        OnUpdate = fkAttr.OnUpdate,
                        ForeignTable = foreignTable,
                        ForeignColumn = foreignColumn
                    });
                }
            }
        }

        #endregion Backing Members
    }
}