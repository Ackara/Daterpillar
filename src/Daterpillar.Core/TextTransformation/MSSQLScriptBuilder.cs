using System;
using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class MSSQLScriptBuilder : IScriptBuilder
    {
        public MSSQLScriptBuilder() : this(MSSQLScriptBuilderSettings.Default, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLScriptBuilder(MSSQLScriptBuilderSettings settings) : this(settings, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLScriptBuilder(MSSQLScriptBuilderSettings settings, ITypeNameResolver typeResolver, string schema = null)
        {
            _settings = settings;
            _typeResolver = typeResolver;
            _schemaName = schema;
        }

        public void Append(string text)
        {
            _text.Append(text);
        }

        public void AppendLine(string text)
        {
            _text.AppendLine(text);
        }

        public void Create(Schema schema)
        {
            _schemaName = (string.IsNullOrEmpty(schema.Name) ? string.Empty : $"[{schema.Name}].");
            foreach (var table in schema.Tables) Create(table);
        }

        public void Create(Table table)
        {
            _text.AppendLine($"CREATE TABLE {_schemaName}[dbo].[{table.Name}]");
            _text.AppendLine("(");

            foreach (var column in table.Columns) AppendToTable(column);
            foreach (var constraint in table.ForeignKeys) AppendToTable(constraint);

            _text.Remove((_text.Length - 3), 3);
            _text.AppendLine();
            _text.AppendLine(");");
            _text.AppendLine();

            foreach (var index in table.Indexes)
            {
                index.Table = table.Name;
                Create(index);
            }

            _text.AppendLine();
        }

        public void Create(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _text.AppendLine($"ALTER TABLE {_schemaName}[dbo].[{column.Table}] ADD [{column.Name}] {dataType}{notNull}{autoIncrement};");
        }

        public void Create(Index index)
        {
            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order}")));
            index.Name = (string.IsNullOrEmpty(index.Name) ? $"{index.Table}_idx{_seed++}" : index.Name).ToLower();

            _text.AppendLine($"CREATE{unique}INDEX [{index.Name}] ON {_schemaName}[dbo].[{index.Table}] ({columns});");
        }

        public void Create(ForeignKey foreignKey)
        {
            if (string.IsNullOrEmpty(foreignKey.Name)) foreignKey.Name = $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}";
            _text.AppendLine($"ALTER TABLE {_schemaName}[dbo].[{foreignKey.LocalTable}]  WITH CHECK ADD  CONSTRAINT [{foreignKey.Name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES {_schemaName}[dbo].[{foreignKey.ForeignTable}] ([{foreignKey.ForeignColumn}]) ON UPDATE {foreignKey.OnUpdate} ON DELETE {foreignKey.OnDelete};");
        }

        public void Drop(Index index)
        {
            _text.AppendLine($"DROP INDEX {_schemaName}[dbo].[{index.Name}];");
        }

        public void Drop(ForeignKey foreignKey)
        {
            _text.AppendLine($"ALTER TABLE {_schemaName}[dbo].[{foreignKey.LocalTable}] DROP CONSTRAINT [{foreignKey.Name}];");
        }

        public void Drop(Schema schema, Column column)
        {
            foreach (var index in schema.GetIndexes()) RemoveAllColumnReferences(index, column.Name);
            foreach (var constraint in schema.GetForeignKeys()) RemoveAllColumnReferences(constraint, column.Table, column.Name);

            _text.AppendLine($"ALTER TABLE {_schemaName}[dbo].[{column.Table}] DROP COLUMN [{column.Name}];");
        }

        public void Drop(Schema schema)
        {
            _text.AppendLine($"DROP DATABASE [{schema.Name}];");
        }

        public void Drop(Table table)
        {
            _text.AppendLine($"DROP TABLE {_schemaName}[dbo].[{table.Name}];");
        }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            throw new NotImplementedException();
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            throw new NotImplementedException();
        }

        public string GetContent()
        {
            return _text.ToString();
        }

        #region Private Members

        private readonly ITypeNameResolver _typeResolver;
        private readonly MSSQLScriptBuilderSettings _settings;
        private readonly StringBuilder _text = new StringBuilder();

        private string _schemaName;
        private int _seed = 1;

        private void AppendToTable(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);

            _text.AppendLine($"\t[{column.Name}] {dataType}{notNull}{autoIncrement},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = (foreignKey.OnUpdateRule != ForeignKeyRule.RESTRICT ? $" ON UPDATE {foreignKey.OnUpdate}" : string.Empty);
            string onDelete = (foreignKey.OnDeleteRule != ForeignKeyRule.RESTRICT ? $" ON DELETE {foreignKey.OnDeleteRule}" : string.Empty);
            foreignKey.Name = (string.IsNullOrEmpty(foreignKey.Name) ? $"{foreignKey.LocalTable}_{foreignKey.LocalColumn}_to_{foreignKey.ForeignTable}_{foreignKey.ForeignColumn}_fkey{_seed++}" : foreignKey.Name).ToLower();

            _text.AppendLine($"\tCONSTRAINT [{foreignKey.Name}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}]([{foreignKey.ForeignColumn}]){onUpdate}{onDelete},");
        }


        private void RemoveAllColumnReferences(Index index, string columnName)
        {
            int columnsRemoved = index.Columns.RemoveAll(x => x.Name == columnName);

            if (columnsRemoved > 0)
            {
                Drop(index);
                Create(index);
            }
        }

        private void RemoveAllColumnReferences(ForeignKey constraint, string tableName, string columnName)
        {
            if ((constraint.LocalTable == tableName && constraint.LocalColumn == columnName)
                || constraint.ForeignTable == tableName && constraint.ForeignColumn == columnName)
            {

            }
        }


        #endregion Private Members
    }
}