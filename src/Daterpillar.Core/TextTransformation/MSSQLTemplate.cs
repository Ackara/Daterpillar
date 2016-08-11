using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class MSSQLTemplate : ITemplate
    {
        public MSSQLTemplate() : this(MSSQLTemplateSettings.Default, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLTemplate(bool dropDatabase) : this(MSSQLTemplateSettings.Default, new MSSQLTypeNameResolver())
        {
            _settings.DropDatabaseIfExist = dropDatabase;
        }

        public MSSQLTemplate(MSSQLTemplateSettings settings) : this(settings, new MSSQLTypeNameResolver())
        {
        }

        public MSSQLTemplate(MSSQLTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            _settings = settings;
            _typeNameResolver = typeResolver;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();
            _schemaName = schema.Name;

            if (_settings.DropDatabaseIfExist)
            {
                _text.AppendLine("BEGIN TRY");
                _text.AppendLine($"DROP DATABASE [{_schemaName}];");
                _text.AppendLine("END TRY");
                _text.AppendLine("BEGIN CATCH");
                _text.AppendLine("END CATCH;");
                _text.AppendLine();
            }

            if (_settings.CreateSchema)
            {
                _text.AppendLine($"CREATE DATABASE [{_schemaName}];");
            }

            if (_settings.UseDatabase)
            {
                _text.AppendLine($"USE [{_schemaName}];");
            }
            _text.AppendLine();

            foreach (var table in schema.Tables)
            {
                Transform(table);
            }

            if (_settings.AddScript)
            {
                _text.AppendLine();
                _text.AppendLine(schema.Script);
            }

            return _text.ToString();
        }

        #region Private Members

        private int _seed;
        private string _schemaName;
        private MSSQLTemplateSettings _settings;
        private ITypeNameResolver _typeNameResolver;
        private StringBuilder _text = new StringBuilder();

        private void Transform(Table table)
        {
            AppendTableComment(table);
            _text.AppendLine($"CREATE TABLE [{_schemaName}].[dbo].[{table.Name}]");
            _text.AppendLine("(");
            foreach (var column in table.Columns)
            {
                Transform(column);
            }

            foreach (var key in table.ForeignKeys)
            {
                Transform(key, table.Name);
            }

            _text.RemoveLastComma();
            _text.AppendLine(");");
            _text.AppendLine();

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Primary))
            {
                TransformPrimaryKey(index, table.Name);
            }

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Index))
            {
                Transform(index, table.Name);
            }
        }

        private void Transform(Column column)
        {
            if (column.AutoIncrement) column.IsNullable = false;

            string dataType = _typeNameResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY IDENTITY(1, 1)" : string.Empty);
            string modifiers = ((column.Modifiers.Count > 0) ? string.Concat(" ", string.Join(" ", column.Modifiers)) : string.Empty);

            _text.AppendLine($"\t[{column.Name}] {dataType}{notNull}{modifiers}{autoIncrement},");
        }

        private void Transform(ForeignKey key, string tableName)
        {
            string onUpdate = (key.OnUpdateRule != ForeignKeyRule.RESTRICT ? $" ON UPDATE {key.OnUpdate}" : string.Empty);
            string onDelete = (key.OnDeleteRule != ForeignKeyRule.RESTRICT ? $" ON DELETE {key.OnDeleteRule}" : string.Empty);
            key.Name = (string.IsNullOrEmpty(key.Name) ? $"{tableName}_{key.LocalColumn}_to_{key.ForeignTable}_{key.ForeignColumn}_fkey{++_seed}" : key.Name).ToLower();
            _text.AppendLine($"\tCONSTRAINT [{key.Name}] FOREIGN KEY ([{key.LocalColumn}]) REFERENCES [{key.ForeignTable}]([{key.ForeignColumn}]){onUpdate}{onDelete},");
        }

        private void TransformPrimaryKey(Index key, string tableName)
        {
            key.Table = (string.IsNullOrEmpty(key.Table) ? tableName : key.Table);
            string name = (string.IsNullOrEmpty(key.Name) ? $"{_schemaName}_{key.Table}_pk" : key.Name);
            string columns = string.Join(", ", key.Columns.Select(x => $"[{x.Name}] {x.Order}"));

            _text.AppendLine($"ALTER TABLE [{_schemaName}].[dbo].[{key.Table}] ADD CONSTRAINT [{name}] PRIMARY KEY ({columns});");
        }

        private void Transform(Index index, string tableName)
        {
            index.Table = (string.IsNullOrEmpty(index.Table) ? tableName : index.Table);
            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => $"[{x.Name}] {x.Order}"));
            index.Name = (string.IsNullOrEmpty(index.Name) ? $"{_schemaName}_{index.Table}_idx{++_seed}" : index.Name).ToLower();

            _text.AppendLine($"CREATE{unique}INDEX [{index.Name}] ON [{_schemaName}].[dbo].[{index.Table}] ({columns});");
        }

        private void AppendTableComment(Table table)
        {
            if (_settings.CommentsEnabled)
            {
                _text.AppendLine("-- ------------------------------------");
                _text.AppendLine($"-- CREATE {table.Name.ToUpper()} TABLE");
                _text.AppendLine("-- ------------------------------------");
            }
        }

        #endregion Private Members
    }
}