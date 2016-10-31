using Gigobyte.Daterpillar.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gigobyte.Daterpillar.TextTransformation
{
    public class CSharpTemplateBuilder : ITemplateBuilder
    {
        public CSharpTemplateBuilder() : this(CSharpTemplateSettings.Default, new CSharpTypeNameResolver())
        {
        }

        public CSharpTemplateBuilder(ITypeNameResolver typeResolver) : this(CSharpTemplateSettings.Default, typeResolver)
        {
        }

        public CSharpTemplateBuilder(CSharpTemplateSettings settings) : this(settings, new CSharpTypeNameResolver())
        {
        }

        public CSharpTemplateBuilder(CSharpTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            Settings = settings;
            _typeResolver = typeResolver;
        }

        public CSharpTemplateSettings Settings { get; private set; }

        public void AlterTable(Column oldColumn, Column newColumn)
        {
            // DO NOTHING
        }

        public void AlterTable(Table oldTable, Table newTable)
        {
            // DO NOTHING
        }

        public void Append(string text)
        {
            _content.Append(text);
        }

        public void AppendLine(string text)
        {
            _content.AppendLine(text);
        }

        public void Create(Index index)
        {
            // DO NOTHING
        }

        public void Create(ForeignKey foreignKey)
        {
            // DO NOTHING
        }

        public void Create(Column column)
        {
            if (Settings.AppendComments)
            {
                string defaultComment = $"\t/// Get or set the {column.Name}.";
                _content.AppendLine($"\t/// <summary>");
                _content.AppendLine((string.IsNullOrWhiteSpace(column.Comment) ? defaultComment : $"\t/// {column.Comment}"));
                _content.AppendLine($"\t/// </summary>");
            }

            string name = column.Name.ToPascalCase(_separators);
            string type = _typeResolver.GetName(column.DataType);
            string virtualKeyword = (Settings.AppendVirtualProperties ? " virtual " : " ");
            string autoIncrement = (column.AutoIncrement ? ", AutoIncrement = true" : string.Empty);
            string key = (IsaKey(column) ? ", Key = true" : string.Empty);

            if (Settings.AppendDataContracts) _content.AppendLine("\t[DataMember]");
            if (Settings.AppendSchemaInformation) _content.AppendLine($"\t[Column(\"{column.Name}\"{key}{autoIncrement})]");
            _content.AppendLine($"\tpublic{virtualKeyword}{type} {name} {{ get; set; }}");
        }

        public void Create(Table table)
        {
            string baseClass = (Settings.AppendSchemaInformation ? $" : {nameof(EntityBase)}" : string.Empty);
            string theNamespace = (!string.IsNullOrEmpty(Settings.Namespace) ? $"(Namespace= \"{Settings.Namespace}\")" : string.Empty);

            if (Settings.AppendSchemaInformation) _content.AppendLine($"[Table(\"{table.Name}\")]");
            if (Settings.AppendDataContracts) _content.AppendLine($"[DataContract{theNamespace}]");
            _content.AppendLine($"public class {table.Name.ToPascalCase(_separators)}{baseClass}");
            _content.AppendLine("{");
            AppendSchemaInformation(table);

            int nColumns = table.Columns.Count;
            for (int i = 0; i < nColumns; i++)
            {
                Create(table.Columns[i]);
                if (i < (nColumns - 1)) _content.AppendLine();
            }

            _content.AppendLine("}");
        }

        public void Create(Schema schema)
        {
            _content.Clear();
            int nTables = schema.Tables.Count;

            for (int i = 0; i < nTables; i++)
            {
                Create(schema.Tables[i]);
                _content.AppendLine();
            }
        }

        public void Drop(Column column)
        {
            // DO NOTHING
        }

        public void Drop(ForeignKey foreignKey)
        {
            // DO NOTHING
        }

        public void Drop(Index index)
        {
            // DO NOTHING
        }

        public void Drop(Table table)
        {
            // DO NOTHING
        }

        public void Drop(Schema schema)
        {
            // DO NOTHING
        }

        public string GetContent()
        {
            return _content.ToString();
        }

        #region Private Members

        private readonly char[] _separators = new char[] { ' ', '\t', '\n', '\r', '_' };
        private ITypeNameResolver _typeResolver;
        private StringBuilder _content = new StringBuilder();

        private void AppendSchemaInformation(Table table)
        {
            if (Settings.AppendSchemaInformation)
            {
                string regionName = "Schema Identifiers";
                _content.AppendLine($"\t#region {regionName}");
                _content.AppendLine();
                _content.AppendLine("\t/// <summary>");
                _content.AppendLine($"\t/// The [{table.Name}] table identifier.");
                _content.AppendLine("\t/// </summary>");
                _content.AppendLine($"\tpublic const string Table = \"{table.Name}\";");
                _content.AppendLine();

                foreach (var column in table.Columns)
                {
                    if (Settings.AppendComments)
                    {
                        _content.AppendLine("\t/// <summary>");
                        _content.AppendLine($"\t/// The [{table.Name}].[{column.Name}] column identifier.");
                        _content.AppendLine("\t/// </summary>");
                    }

                    _content.AppendLine($"\tpublic const string {column.Name.ToPascalCase(_separators)}Column = \"{column.Name}\";");
                    _content.AppendLine();
                }

                _content.AppendLine($"\t#endregion {regionName}");
                _content.AppendLine();
            }
        }

        private void AppendColumns(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                Create(column);
            }
        }

        private bool IsaKey(Column column)
        {
            if (column.AutoIncrement) return true;
            else foreach (var index in column.TableRef.Indexes.Where(x => x.Type == IndexType.PrimaryKey))
                {
                    foreach (var idxColumn in index.Columns)
                    {
                        if (idxColumn.Name == idxColumn.Name) return true;
                    }
                }

            return false;
        }

        #endregion Private Members
    }
}