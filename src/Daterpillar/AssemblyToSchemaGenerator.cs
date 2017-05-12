using Ackara.Daterpillar.TypeResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Ackara.Daterpillar
{
    /// <summary>
    /// Contains methods to convert an <see cref="Assembly"/> to a <see cref="Schema"/>.
    /// </summary>
    public static class AssemblyToSchemaGenerator
    {
        /// <summary>
        /// Converts the specified <see cref="Assembly"/> to an <see cref="Schema"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="syntax">The syntax.</param>
        /// <returns>A <see cref="Schema"/> object built using types decorated with <see cref="Ackara.Daterpillar"/> attributes.</returns>
        public static Schema ToSchema(this Assembly assembly, Syntax syntax = Syntax.Generic)
        {
            var schema = new Schema();

            var schemaTypes = (
                from t in assembly.ExportedTypes
                where t.GetTypeInfo().GetCustomAttribute<TableAttribute>() != null
                select t.GetTypeInfo());

            foreach (var typeInfo in schemaTypes)
            {
                if (typeInfo.IsEnum)
                {
                    ConvertToTable(typeInfo, syntax, out Table table, out Script script);
                    schema.Add(table);
                    schema.Add(script);
                }
                else
                {
                    Table table = ConvertToTable(typeInfo, schemaTypes);
                    schema.Add(table);
                }
            }
            schema.AssignParentNodes();
            return schema;
        }

        internal static string Escape(this string name, Syntax syntax)
        {
            switch (syntax)
            {
                default:
                case Syntax.Generic:
                    return name;

                case Syntax.MySQL:
                    return $"`{name}`";

                case Syntax.MSSQL:
                case Syntax.SQLite:
                    return $"[{name}]";
            }
        }

        private static void ConvertToTable(TypeInfo enumType, Syntax syntax, out Table table, out Script script)
        {
            string tableName = enumType.GetCustomAttribute<TableAttribute>().Name;

            table = new Table(tableName);
            table.Columns.Add(new Column("Id", new DataType("int")));
            table.Columns.Add(new Column("Name", new DataType("varchar", 32)));
            table.Indexes.Add(new Index(IndexType.PrimaryKey, new ColumnName("Id")));
            table.Indexes.Add(new Index(IndexType.Index, true, new ColumnName("Name")));
            // --- script ---
            var content = new StringBuilder();
            content.Append($"INSERT INTO {Escape(tableName, syntax)} ({Escape("Id", syntax)}, {Escape("Name", syntax)}) VALUES");
            foreach (var field in enumType.DeclaredFields.Where(x => x.FieldType == enumType.AsType()))
            {
                var attribute = field.GetCustomAttribute<EnumValueAttribute>();
                string name = (attribute == null ? field.Name : attribute.Name);
                int value = Convert.ToInt32(field.GetValue(null));

                content.Append($" ('{value}', '{name}'),");
            }

            content.Remove((content.Length - 1), 1);
            content.Append(";");

            script = new Script(content, syntax, name: $"{tableName} seed data");
        }

        private static Table ConvertToTable(TypeInfo typeInfo, IEnumerable<TypeInfo> schemaTypes)
        {
            var tableAttribute = typeInfo.GetCustomAttribute<TableAttribute>();
            var table = new Table(tableAttribute?.Name ?? typeInfo.Name);

            // Columns
            AddColumns(table, typeInfo, out IEnumerable<PropertyInfo> columns);
            AddIndexes(table, columns);
            AddForeignKeys(table, columns, schemaTypes);

            return table;
        }

        private static void AddColumns(Table table, TypeInfo typeInfo, out IEnumerable<PropertyInfo> columns)
        {
            columns = from p in typeInfo.DeclaredProperties
                      where p.GetCustomAttribute<ColumnAttribute>() != null
                      select p;

            foreach (var type in columns)
            {
                var column = type.GetCustomAttribute<ColumnAttribute>();
                DataType dataType = (string.IsNullOrEmpty(column.DataType.Name) ? CSharpTypeResolver.GetDataType(type.PropertyType) : column.DataType);
                table.Columns.Add(new Column((column.Name ?? type.Name), dataType, column.AutoIncrement, column.Nullable, column.DefaultValue));
            }
        }

        private static void AddIndexes(Table table, IEnumerable<PropertyInfo> columns)
        {
            var indexes = from prop in columns
                          where prop.GetCustomAttribute<IndexAttribute>() != null
                          group prop by prop?.GetCustomAttribute<IndexAttribute>()?.GroupName;

            foreach (var group in indexes)
            {
                var composite = new Index();
                var names = new List<ColumnName>();

                foreach (var type in group)
                {
                    var attribute = type.GetCustomAttribute<IndexAttribute>();
                    var columnName = (type.GetCustomAttribute<ColumnAttribute>().Name ?? type.Name);

                    if (attribute.Type == IndexType.Index && group.Key == null)
                    {
                        table.Indexes.Add(new Index(attribute.Type, attribute.Unique, new ColumnName(columnName)));
                    }
                    else
                    {
                        names.Add(new ColumnName(columnName));
                        composite.Type = attribute.Type;
                        composite.Columns = names.ToArray();
                    }
                }

                if (composite.Columns.Length > 0) table.Indexes.Add(composite);
            }
        }

        private static void AddForeignKeys(Table table, IEnumerable<PropertyInfo> columns, IEnumerable<TypeInfo> schemaTypes)
        {
            var foreignKeys = from col in columns
                              where col.GetCustomAttribute<ForeignKeyAttribute>() != null
                              select col;

            foreach (var type in foreignKeys)
            {
                var attribute = type.GetCustomAttribute<ForeignKeyAttribute>();
                string localColumn = type.GetCustomAttribute<ColumnAttribute>().Name ?? type.Name;

                var referencedTableType = (from x in schemaTypes
                                           where x.Name == attribute.ForeignTable
                                           select x).FirstOrDefault();

                string foreignTableName, foreignColumnName;
                if (referencedTableType == null)
                {
                    foreignTableName = attribute.ForeignTable;
                    foreignColumnName = attribute.ForeignColumn;
                }
                else
                {
                    var referencedColumnType = (from prop in referencedTableType.DeclaredProperties
                                                where prop.Name == attribute.ForeignColumn && prop.GetCustomAttribute<ColumnAttribute>() != null
                                                select prop).FirstOrDefault();

                    foreignTableName = (referencedTableType.GetCustomAttribute<TableAttribute>().Name ?? referencedTableType.Name);
                    foreignColumnName = (referencedColumnType == null ? attribute.ForeignColumn : (referencedColumnType.GetCustomAttribute<ColumnAttribute>().Name ?? referencedColumnType.Name));
                }

                table.ForeignKeys.Add(new ForeignKey(localColumn, foreignTableName, foreignColumnName, attribute.OnUpdate, attribute.OnDelete));
            }
        }
    }
}