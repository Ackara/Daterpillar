using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class TSQLScriptBuilder : IScriptBuilder
    {
        public TSQLScriptBuilder() : this(ScriptBuilderSettings.Default, new TSQLTypeNameResolver())
        {
        }

        public TSQLScriptBuilder(ITypeNameResolver typeResolver) : this(ScriptBuilderSettings.Default, typeResolver)
        {
        }

        public TSQLScriptBuilder(ScriptBuilderSettings settings) : this(settings, new TSQLTypeNameResolver())
        {
        }

        public TSQLScriptBuilder(ScriptBuilderSettings settings, ITypeNameResolver typeResolver)
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

            _script.AppendLine($"IF DB_ID('{schema.Name}') IS NULL CREATE DATABASE [{schema.Name}];");
            _script.AppendLine($"USE [{schema.Name}];");
            _script.AppendLine();

            foreach (var table in schema.Tables) Create(table);

            if (_settings.AppendScripts)
            {
                lineBreak = "-- =================";
                _script.AppendLine(lineBreak);
                _script.AppendLine($"-- SCRIPTS (000)");
                _script.AppendLine(lineBreak);
                _script.AppendLine();

                _script.AppendLine(schema.Script);
            }
        }

        public void Create(Table table)
        {
            string schema = table.SchemaRef.Name;
            _script.AppendLine($"IF OBJECT_ID('{schema}.dbo.{table.Name}') IS NULL CREATE TABLE [{schema}].[dbo].[{table.Name}]");
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
            string table = index.TableRef.Name;
            string schema = index.TableRef.SchemaRef.Name;

            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order}")));
            index.Name = (string.IsNullOrEmpty(index.Name) ? $"{index.Table}_idx{_seed++}" : index.Name).ToLower();

            _script.AppendLine($"IF NOT EXISTS(SELECT * FROM [sys].[indexes] WHERE [object_id]=OBJECT_ID('{schema}.dbo.{table}') AND [name]='{index.Name}') CREATE{unique}INDEX [{index.Name}] ON [{index.TableRef.SchemaRef.Name}].[dbo].[{index.Table}] ({columns});");
        }

        public void Create(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string schema = foreignKey.TableRef.SchemaRef.Name;

            if (string.IsNullOrEmpty(foreignKey.Name)) foreignKey.Name = $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}";
            _script.AppendLine($"IF NOT EXISTS (SELECT * FROM [sys].[foreign_keys] WHERE [name]='{foreignKey.Name}' AND [parent_object_id]=OBJECT_ID('{schema}.dbo.{table}')) ALTER TABLE [{schema}].[dbo].[{table}] WITH CHECK ADD CONSTRAINT [{foreignKey.Name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{schema}].[dbo].[{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate.ToText()} ON DELETE {foreignKey.OnDelete.ToText()};");
        }

        public void Drop(Schema schema)
        {
            _script.AppendLine($"IF DB_ID('{schema.Name}') IS NOT NULL DROP DATABASE [{schema.Name}];");
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
            string table = foreignKey.LocalTable;
            string schema = foreignKey.TableRef.SchemaRef.Name;

            if (string.IsNullOrEmpty(foreignKey.Name)) foreignKey.Name = $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}";
            _script.AppendLine($"IF EXISTS (SELECT * FROM [sys].[foreign_keys] WHERE [name]='{foreignKey.Name}' AND [parent_object_id]=OBJECT_ID('{schema}.dbo.{table}')) ALTER TABLE [{schema}].[dbo].[{table}] DROP CONSTRAINT [{foreignKey.Name}];");
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

        public void Clear()
        {
            _script.Clear();
        }

        #region Private Members

        private readonly ITypeNameResolver _typeResolver;
        private readonly ScriptBuilderSettings _settings;
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