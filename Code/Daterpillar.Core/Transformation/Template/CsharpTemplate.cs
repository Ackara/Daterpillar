using Ackara.Daterpillar.Annotation;
using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public class CsharpTemplate : ITemplate
    {
        public CsharpTemplate() : this(CsharpTemplateSettings.Default, new CsharpTypeNameResolver())
        {
        }

        public CsharpTemplate(CsharpTemplateSettings settings) : this(settings, new CsharpTypeNameResolver())
        {
        }

        public CsharpTemplate(CsharpTemplateSettings settings, ITypeNameResolver typeResolver)
        {
            _settings = settings;
            _typeResolver = typeResolver;
        }

        public string Transform(Schema schema)
        {
            _text.Clear();

            int nTables = schema.Tables.Count;
            for (int i = 0; i < nTables; i++)
            {
                Transform(schema.Tables[i]);
                if (i < (nTables - 1)) _text.AppendLine();
            }

            return _text.ToString().Trim();
        }

        #region Private Members

        private Table _currentTable;
        private ITypeNameResolver _typeResolver;
        private CsharpTemplateSettings _settings;
        private StringBuilder _text = new StringBuilder();

        private static string GetAttributeShortName(string attributeName)
        {
            return attributeName.Replace("Attribute", "");
        }

        private void GenerateConstants(Table table)
        {
            if (_settings.SchemaAnnotationsEnabled)
            {
                _text.AppendLine("\t#region Constants");
                _text.AppendLine();

                if (_settings.CommentsEnabled)
                {
                    _text.AppendLine("\t/// <summary>");
                    _text.AppendLine($"\t/// The {table.Name} table identifier.");
                    _text.AppendLine("\t/// </summary>");
                }
                _text.AppendLine($"\tpublic const string Table = \"{table.Name}\";");
                _text.AppendLine();

                foreach (var column in table.Columns)
                {
                    if (_settings.CommentsEnabled)
                    {
                        _text.AppendLine("\t/// <summary>");
                        _text.AppendLine($"\t/// The [{table.Name}].[{column.Name}] column identifier.");
                        _text.AppendLine("\t/// </summary>");
                    }

                    _text.AppendLine($"\tpublic const string {column.Name.ToPascalCase(' ', '_')}Column = \"{column.Name}\";");
                    _text.AppendLine();
                }

                _text.AppendLine("\t#endregion Constants");
                _text.AppendLine();
            }
        }

        private void Transform(Table table)
        {
            _currentTable = table;

            AppendComment(table);
            AppendDataContract(table);
            AppendSchemaAttribute(table);

            string baseType = _settings.SchemaAnnotationsEnabled ? (" : " + typeof(Data.EntityBase).Name) : string.Empty;
            _text.AppendLine($"public partial class {table.Name.ToPascalCase(' ', '_')}{baseType}");
            _text.AppendLine("{");

            GenerateConstants(table);

            int nColumns = table.Columns.Count;
            for (int i = 0; i < nColumns; i++)
            {
                Transform(table.Columns[i]);
                if (i < (nColumns - 1)) _text.AppendLine();
            }

            _text.AppendLine("}");
        }

        private void Transform(Column column)
        {
            AppendComment(column);
            AppendDataContract(column);
            AppendSchemaAttribute(column);

            string dataType = _typeResolver.GetName(column.DataType);
            string virtualModifier = _settings.VirtualPropertiesEnabled ? " virtual " : " ";
            _text.AppendLine($"\tpublic{virtualModifier}{dataType} {column.Name.ToPascalCase(' ', '_')} {{ get; set; }}");
        }

        private void AppendComment(Table table)
        {
            if (_settings.CommentsEnabled)
            {
                if (string.IsNullOrWhiteSpace(table.Comment))
                {
                    _text.AppendLine("/// <summary>");
                    _text.AppendLine($"/// Represents the [{table.Name}] table.");
                    _text.AppendLine("/// </summary>");
                }
                else
                {
                    _text.AppendLine("/// <summary>");
                    _text.AppendLine($"/// {(table.Comment.Trim()).AppendPeriod()}");
                    _text.AppendLine("/// </summary>");
                }
            }
        }

        private void AppendDataContract(Table table)
        {
            if (_settings.DataContractsEnabled)
            {
                string nameSpace = string.IsNullOrEmpty(_settings.Namespace) ? "" : $"(Namespace = \"{_settings.Namespace}\")";
                _text.AppendLine($"[DataContract{nameSpace}]");
            }
        }

        private void AppendSchemaAttribute(Table table)
        {
            if (_settings.SchemaAnnotationsEnabled)
            {
                string attribute = GetAttributeShortName(typeof(TableAttribute).Name);
                _text.AppendLine($"[{attribute}(\"{table.Name}\")]");
            }
        }

        private void AppendComment(Column column)
        {
            if (_settings.CommentsEnabled)
            {
                if (string.IsNullOrWhiteSpace(column.Comment))
                {
                    _text.AppendLine("\t/// <summary>");
                    _text.AppendLine($"\t/// Get or set the [{_currentTable.Name}].[{column.Name}] column value.");
                    _text.AppendLine("\t/// </summary>");
                }
                else
                {
                    _text.AppendLine("\t/// <summary>");
                    _text.AppendLine($"\t/// {(column.Comment.Trim()).AppendPeriod()}");
                    _text.AppendLine("\t/// </summary>");
                }
            }
        }

        private void AppendDataContract(Column column)
        {
            if (_settings.DataContractsEnabled)
            {
                _text.AppendLine($"\t[DataMember]");
            }
        }

        private void AppendSchemaAttribute(Column column)
        {
            if (_settings.SchemaAnnotationsEnabled)
            {
                string attribute = GetAttributeShortName(typeof(ColumnAttribute).Name);
                bool isKey = _currentTable.Indexes.IsKey(column.Name);
                string key = isKey ? ", IsKey = true" : string.Empty;
                string autoIncrement = column.AutoIncrement ? ", AutoIncrement = true" : string.Empty;

                _text.AppendLine($"\t[{attribute}(\"{column.Name}\"{key}{autoIncrement})]");
            }
        }

        #endregion Private Members
    }
}