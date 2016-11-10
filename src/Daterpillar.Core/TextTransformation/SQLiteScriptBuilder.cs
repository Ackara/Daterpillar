using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class SQLiteScriptBuilder : IScriptBuilder
    {
        public SQLiteScriptBuilder() : this(ScriptBuilderSettings.Default, new SQLiteTypeNameResolver())
        {
        }

        public SQLiteScriptBuilder(ITypeNameResolver typeResolver) : this(ScriptBuilderSettings.Default, typeResolver)
        {
        }

        public SQLiteScriptBuilder(ScriptBuilderSettings settings) : this(settings, new SQLiteTypeNameResolver())
        {
        }

        public SQLiteScriptBuilder(ScriptBuilderSettings settings, ITypeNameResolver typeResolver)
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
            _script.AppendLine($"CREATE TABLE IF NOT EXISTS [{table.Name}]");
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
            string table = column.TableRef.Name;
            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = (column.IsNullable ? string.Empty : " NOT NULL");
            string defaultValue = (!column.IsNullable ? $" DEFAULT '{column.DefaultValue ?? string.Empty}'" : string.Empty);
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTOINCREMENT" : string.Empty);

            _script.AppendLine($"ALTER TABLE [{table}] ADD COLUMN [{column.Name}] {dataType}{notNull}{defaultValue}{autoIncrement};");
        }

        public void Create(Index index)
        {
            string table = index.TableRef.Name;
            string unique = (index.Unique ? " UNIQUE " : " ");
            string columns = string.Join(", ", index.Columns.Select(x => ($"[{x.Name}] {x.Order}")));
            
            _script.AppendLine($"CREATE{unique}INDEX IF NOT EXISTS [{index.GetName(_seed++)}] ON [{index.Table}] ({columns});");
        }

        public void Create(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string tempName = $"{table}_temp_table";
            string schema = foreignKey.TableRef.SchemaRef.Name;
            var columns = string.Join(", ", foreignKey.TableRef.Columns.Select(x => $"[{x.Name}]"));
            var clone = foreignKey.TableRef.Clone();
            clone.ForeignKeys.Add(foreignKey);

            _script.AppendLine($"/* ADD ([{table}].[{foreignKey.Name}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table}];");
            _script.AppendLine($"DROP TABLE [{table}];");
            _script.AppendLine();

            Create(clone);

            _script.AppendLine($"INSERT INTO [{table}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
        }

        public void Drop(Schema schema)
        {
            // DO NOTHING
        }

        public void Drop(Table table)
        {
            _script.AppendLine($"DROP TABLE IF EXISTS [{table.Name}];");
        }

        public void Drop(Column column)
        {
            string table = column.TableRef.Name;
            string tempName = $"{table}_temp_table";
            Schema schema = column.TableRef.SchemaRef;
            var copyOfTable = column.TableRef.Clone();
            copyOfTable.RemoveColumn(column.Name);
            var columns = string.Join(", ", copyOfTable.Columns.Select(x => $"[{x.Name}]"));

            foreach (var constraint in schema.GetForeignKeys()) RemoveAllReferencesToColumn(constraint, table, column.Name);
            foreach (var index in schema.GetIndexes()) RemoveAllReferencesToColumn(index, column.Name);

            _script.AppendLine($"/* DROP ([{table}].[{column.Name}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table}];");
            _script.AppendLine($"DROP TABLE [{table}];");

            Create(copyOfTable);

            _script.AppendLine($"INSERT INTO [{table}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
        }

        public void Drop(Index index)
        {
            string table = index.TableRef.Name;
            _script.AppendLine($"DROP INDEX IF EXISTS [{index.Name}];");
        }

        public void Drop(ForeignKey foreignKey)
        {
            string table = foreignKey.LocalTable;
            string tempName = $"{table}_temp_table";
            string schema = foreignKey.TableRef.SchemaRef.Name;
            var copyOfTable = foreignKey.TableRef.Clone();
            copyOfTable.RemoveForeignKey(foreignKey.Name);
            var columns = string.Join(", ", foreignKey.TableRef.Columns.Select(x => $"[{x.Name}]"));

            _script.AppendLine($"/* DROP ([{table}].[{foreignKey.Name}]) SCRIPT */");
            _script.AppendLine("PRAGMA foreign_keys = 0;");
            _script.AppendLine($"CREATE TABLE [{tempName}] AS SELECT * FROM [{table}];");
            _script.AppendLine($"DROP TABLE [{table}];");

            Create(copyOfTable);

            _script.AppendLine($"INSERT INTO [{table}] ({columns}) SELECT {columns} FROM [{tempName}];");
            _script.AppendLine($"DROP TABLE [{tempName}];");
            _script.AppendLine("PRAGMA foreign_keys = 1;");
            _script.AppendLine("/* END SCRIPT */");
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            // Do nothing.
        }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            Drop(oldColumn);
            Create(newColumn);
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
            string dataType = _typeResolver.GetName(column.DataType);
            string notNull = ((column.IsNullable && !column.AutoIncrement) ? string.Empty : " NOT NULL");
            string autoIncrement = (column.AutoIncrement ? $" PRIMARY KEY AUTOINCREMENT" : string.Empty);
            string defaultValue = (column.DefaultValue == null ? string.Empty : $" DEFAULT '{column.DefaultValue}'");

            _script.AppendLine($"\t[{column.Name}] {dataType}{notNull}{defaultValue}{autoIncrement},");
        }

        private void AppendToTable(ForeignKey foreignKey)
        {
            string onUpdate = (foreignKey.OnUpdate != ForeignKeyRule.RESTRICT ? $" ON UPDATE {foreignKey.OnUpdate}" : string.Empty);
            string onDelete = (foreignKey.OnDelete != ForeignKeyRule.RESTRICT ? $" ON DELETE {foreignKey.OnDelete}" : string.Empty);
            
            _script.AppendLine($"\tCONSTRAINT [{foreignKey.GetName(_seed++)}] FOREIGN KEY ([{foreignKey.LocalColumn}]) REFERENCES [{foreignKey.ForeignTable}]([{foreignKey.ForeignColumn}]){onUpdate}{onDelete},");
        }

        private void RemoveAllReferencesToColumn(Index index, string columnName)
        {
            Index clone = index.Clone();
            bool indexColumnsWereRemoved = (clone.Columns.RemoveAll(x => x.Name == columnName) > 0);
            if (indexColumnsWereRemoved)
            {
                bool shouldRemoveIndex = (clone.Columns.Count == 0);

                if (shouldRemoveIndex)
                {
                    Drop(clone);
                }
                else /* All of the index columns were NOT removed */
                {
                    Drop(clone);
                    Create(clone);
                }
            }
        }

        private void RemoveAllReferencesToColumn(ForeignKey constraint, string tableName, string columnName)
        {
            if ((constraint.LocalTable == tableName && constraint.LocalColumn == columnName)
                || constraint.ForeignTable == tableName && constraint.ForeignColumn == columnName)
            {
                Drop(constraint);
                _script.AppendLine();
            }
        }

        #endregion Private Members
    }
}