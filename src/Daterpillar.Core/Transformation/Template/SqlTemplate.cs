using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class SqlTemplate : ITemplate
    {
        public SqlTemplate() : this(SqlTemplateSettings.Default, new SqlTypeNameResolver())
        {
        }

        public SqlTemplate(bool dropDatabase) : this(SqlTemplateSettings.Default, new SqlTypeNameResolver())
        {
            _settings.DropDatabaseIfExist = dropDatabase;
        }

        public SqlTemplate(SqlTemplateSettings settings) : this(settings, new SqlTypeNameResolver())
        {
        }

        public SqlTemplate(SqlTemplateSettings settings, ITypeNameResolver typeResolver)
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
        private SqlTemplateSettings _settings;
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

            foreach (var index in table.Indexes.Where(x => x.IndexType == IndexType.Primary))
            {
                Transform(index, table.Name);
            }
        }

        private void Transform(Column column)
        {
            string dataType = _typeNameResolver.GetName(column.DataType);
            string modifiers = string.Join(" ", column.Modifiers);

            int seed = column.DataType.Scale;
            int increment = column.DataType.Precision;
            string autoIncrement = (column.AutoIncrement ? $" IDENTITY({(seed == 0 ? 1 : seed)}, {(increment == 0 ? 1 : increment)})" : string.Empty);

            _text.AppendLine($"\t[{column.Name}] {dataType} {modifiers}{autoIncrement},");
        }

        private void Transform(ForeignKey key, string tableName)
        {
            string name = (string.IsNullOrEmpty(key.Name) ? $"{tableName}_{key.LocalColumn}_to_{key.ForeignTable}_{key.ForeignColumn}_fkey{++_seed}" : key.Name).ToLower();
            _text.AppendLine($"\tCONSTRAINT [{name}] FOREIGN KEY ([{key.LocalColumn}]) REFERENCES [{key.ForeignTable}]([{key.ForeignColumn}]) ON UPDATE {key.OnUpdate} ON DELETE {key.OnDelete},");
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
            string unique = (index.Unique ? "unique" : " ");
            string columns = string.Join(", ", index.Columns.Select(x => $"[{x.Name}] {x.Order}"));
            string name = (string.IsNullOrEmpty(index.Name) ? $"{_schemaName}_{index.Table}_idx{++_seed}" : index.Name).ToLower();

            _text.AppendLine($"CREATE{unique}INDEX [{name}] ON [{_schemaName}].[dbo].[{index.Table}] ({columns});");
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