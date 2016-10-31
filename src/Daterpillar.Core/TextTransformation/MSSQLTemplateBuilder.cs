using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class MSSQLTemplateBuilder : ITemplateBuilder
    {
        public MSSQLTemplateBuilder() : this(TemplateBuilderSettings.Default, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLTemplateBuilder(TemplateBuilderSettings settings) : this(settings, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLTemplateBuilder(TemplateBuilderSettings settings, ITypeNameResolver typeResolver)
        {
            _settings = settings;
            _typeResolver = typeResolver;
        }

        public void Append(string text)
        {
            _script.Append(text);
        }

        public void AppendLine(string text)
        {
            _script.AppendLine(text);
        }

        public void Create(Schema schema)
        {
            string lineBreak = "-- ======================================================================";

            _script.AppendLine(lineBreak);
            _script.AppendLine("-- NAME:");
            _script.AppendLine($"-- {schema.Name}");
            _script.AppendLine();
            _script.AppendLine("-- DESCRIPTION:");
            _script.AppendLine($"-- {schema.Description}");
            _script.AppendLine();
            _script.AppendLine("-- AUTHOR:");
            _script.AppendLine($"-- {schema.Author}");
            _script.AppendLine();
            _script.AppendLine("-- DATE:");
            _script.AppendLine($"-- {schema.CreatedOn.ToString("ddd dd, yyyy hh:mm tt")}");
            _script.AppendLine(lineBreak);
            _script.AppendLine();

            _script.AppendLine($"CREATE DATABASE [{schema.Name}];");
            _script.AppendLine();

            foreach (var table in schema.Tables) Create(table);

            lineBreak = "-- =================";
            _script.AppendLine(lineBreak);
            _script.AppendLine($"-- SCRIPTS (000)");
            _script.AppendLine(lineBreak);
            _script.AppendLine();

            _script.AppendLine(schema.Script);
        }

        public void Create(Table table)
        {
            _script.AppendLine($"CREATE TABLE [{table.SchemaRef.Name}].[dbo].[{table.Name}]");
            _script.AppendLine("(");

            foreach (var column in table.Columns) AppendToTable(column);
            foreach (var constraint in table.ForeignKeys) AppendToTable(constraint);

            _script.Remove((_script.Length - 3), 3);
            _script.AppendLine();
            _script.AppendLine(");");
            _script.AppendLine();

            foreach (var index in table.Indexes)
            {
                index.Table = table.Name;
                Create(index);
            }

            _script.AppendLine();
        }

        public void Create(Column column)
        {
            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{column.TableRef.SchemaRef.Name}].[dbo].[{column.TableRef.Name}] ADD [{column.Name}] {dataType}{notNull}{autoIncrement};");
        }

        public void Create(Index index)
        {
            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order}")));
            index.Name = (string.IsNullOrEmpty(index.Name) ? $"{index.Table}_idx{_seed++}" : index.Name).ToLower();

            _script.AppendLine($"CREATE{unique}INDEX [{index.Name}] ON [{index.TableRef.SchemaRef.Name}].[dbo].[{index.Table}] ({columns});");
        }

        public void Create(ForeignKey foreignKey)
        {
            if (string.IsNullOrEmpty(foreignKey.Name)) foreignKey.Name = $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}";
            _script.AppendLine($"ALTER TABLE [{foreignKey.TableRef.SchemaRef.Name}].[dbo].[{foreignKey.LocalTable}]  WITH CHECK ADD  CONSTRAINT [{foreignKey.Name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.TableRef.SchemaRef.Name}].[dbo].[{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete};");
        }

        public void Drop(Schema schema)
        {
            _script.AppendLine($"DROP DATABASE [{schema.Name}];");
        }

        public void Drop(Table table)
        {
            _script.AppendLine($"DROP TABLE [{table.SchemaRef.Name}].[dbo].[{table.Name}];");
        }

        public void Drop(Column column)
        {
            var schema = column.TableRef.SchemaRef;

            foreach (var index in schema.GetIndexes()) RemoveAllReferencesToColumn(index, column.Name);
            foreach (var constraint in schema.GetForeignKeys()) RemoveAllReferencesToColumn(constraint, column.TableRef.Name, column.Name);

            _script.AppendLine($"ALTER TABLE [{column.TableRef.SchemaRef.Name}].[dbo].[{column.TableRef.Name}] DROP COLUMN [{column.Name}];");
        }

        public void Drop(Index index)
        {
            _script.AppendLine($"DROP INDEX [{index.TableRef.SchemaRef.Name}].[dbo].[{index.Name}];");
        }

        public void Drop(ForeignKey foreignKey)
        {
            _script.AppendLine($"ALTER TABLE [{foreignKey.TableRef.SchemaRef.Name}].[dbo].[{foreignKey.LocalTable}] DROP CONSTRAINT [{foreignKey.Name}];");
        }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            if (newColumn.AutoIncrement) newColumn.IsNullable = false;

            string dataType = _typeResolver.GetName(newColumn.DataType);
            string notNull = (newColumn.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (newColumn.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{oldColumn.TableRef.Name}] ALTER COLUMN [{oldColumn.Name}] {dataType}{notNull}{autoIncrement};");
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            // Do nothing.
        }

        public string GetContent()
        {
            return _script.ToString();
        }

        #region Private Members

        private readonly ITypeNameResolver _typeResolver;
        private readonly TemplateBuilderSettings _settings;
        private readonly StringBuilder _script = new StringBuilder();

        private int _seed = 1;

        private void AppendToTable(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _script.AppendLine($"\t[{column.Name}] {dataType}{notNull}{autoIncrement},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = (foreignKey.OnUpdate != ForeignKeyRule.RESTRICT ? $" ON UPDATE {foreignKey.OnUpdate}" : string.Empty);
            string onDelete = (foreignKey.OnDelete != ForeignKeyRule.RESTRICT ? $" ON DELETE {foreignKey.OnDelete}" : string.Empty);
            foreignKey.Name = (string.IsNullOrEmpty(foreignKey.Name) ? $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}" : foreignKey.Name).ToLower();

            _script.AppendLine($"\tCONSTRAINT [{foreignKey.Name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}]([{foreignKey.ForeignColumn}]){onUpdate}{onDelete},");
        }

        private void RemoveAllReferencesToColumn(Index index, string columnName)
        {
            int columnsRemoved = index.Columns.RemoveAll(x => x.Name == columnName);

            if (columnsRemoved > 0)
            {
                Drop(index);
                Create(index);
            }
        }

        private void RemoveAllReferencesToColumn(ForeignKey constraint, string tableName, string columnName)
        {
            if ((constraint.LocalTable == tableName && constraint.LocalColumn == columnName)
                || constraint.ForeignTable == tableName && constraint.ForeignColumn == columnName)
            {
                Drop(constraint);
            }
        }

        #endregion Private Members
    }
}