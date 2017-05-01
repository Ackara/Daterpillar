using Ackara.Daterpillar.TypeResolvers;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Provides functions used for building C# scripts.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Scripting.IScriptBuilder" />
    public class CSharpScriptBuilder : IScriptBuilder
    {
        public CSharpScriptBuilder() : this(new CSharpScriptBuilderSettings(), new CSharpTypeResolver())
        { }

        public CSharpScriptBuilder(CSharpScriptBuilderSettings settings) : this(settings, new CSharpTypeResolver())
        { }

        public CSharpScriptBuilder(CSharpScriptBuilderSettings settings, ITypeResolver resolver)
        {
            _content = new StringBuilder();
            _typeResolver = resolver;
            Settings = settings;
        }

        public CSharpScriptBuilderSettings Settings;

        public int Length
        {
            get { return _content.Length; }
        }

        public bool IsEmpty
        {
            get { return _content.Length == 0; }
        }

        public IScriptBuilder Append(string text)
        {
            _content.Append(text);
            return this;
        }

        public IScriptBuilder AppendLine(string text)
        {
            _content.AppendLine(text);
            return this;
        }

        public IScriptBuilder Append(Schema schema)
        {
            foreach (var table in schema.Tables) Append(table);
            return this;
        }

        public IScriptBuilder Append(Table table)
        {
            //Comment(table);
            AppendAttributes(table);
            _content.AppendLine($"public partial class {table.Name.ToPascalCase()}");
            _content.AppendLine("{");
            //AppendConstants(table);
            foreach (var column in table.Columns) Append(column);
            _content.AppendLine("}");
            _content.AppendLine();
            return this;
        }

        public IScriptBuilder Append(Column column)
        {
            string name = column.Name.ToPascalCase();
            string type = _typeResolver.GetTypeName(column.DataType);
            string virtualKeyword = (Settings.UseVirtualProperties ? " virtual " : " ");
            string autoIncrement = (column.AutoIncrement ? ", AutoIncrement = true" : string.Empty);

            //Comment(column);
            AppendAttributes(column);
            _content.AppendLine($"\tpublic{virtualKeyword}{type} {name} {{ get; set; }}");
            _content.AppendLine();
            return this;
        }

        public IScriptBuilder Append(Index index)
        {
            return this;
        }

        public IScriptBuilder Append(ForeignKey foreignKey)
        {
            return this;
        }

        public IScriptBuilder Append(IDictionary<string, int> valuePairs)
        {
            return this;
        }

        public IScriptBuilder Remove(Schema schema)
        {
            return this;
        }

        public IScriptBuilder Remove(Table table)
        {
            return this;
        }

        public IScriptBuilder Remove(Column column)
        {
            return this;
        }

        public IScriptBuilder Remove(Index index)
        {
            return this;
        }

        public IScriptBuilder Remove(ForeignKey foreignKey)
        {
            return this;
        }

        public IScriptBuilder Update(Table oldTable, Table newTable)
        {
            return this;
        }

        public IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            return this;
        }

        public string GetContent()
        {
            return GetContent(string.Empty);
        }

        public string GetContent(string header)
        {
            return string.Concat(header, _content);
        }

        public void Clear()
        {
            _content.Clear();
        }

        #region Private Members

        private readonly StringBuilder _content;
        private readonly ITypeResolver _typeResolver;

        private void AppendConstants(Table table)
        {
            if (Settings.AddConstants)
            {
                string regionName = "SCHEMA NAMES";
                _content.AppendLine($"\t#region {regionName}");
                _content.AppendLine();
                _content.AppendLine($"\tpublic const string Table = \"{table.Name}\";");
                foreach (var column in table.Columns)
                {
                    _content.AppendLine();
                }

                _content.AppendLine($"\t#endregion {regionName}");
                _content.AppendLine();
            }
        }

        private void AppendAttributes(Table table)
        {
            if (Settings.AddSchemaAttributes)
            {
                _content.AppendLine($"[Table(\"{table.Name}\")]");
            }
        }

        private void AppendAttributes(Column column)
        {
            if (Settings.AddSchemaAttributes)
            {
                // Column
                string nullable = (column.IsNullable ? $", {nameof(ColumnAttribute.Nullable)} = true" : string.Empty);
                string autoIncrement = (column.AutoIncrement ? $", {nameof(ColumnAttribute.AutoIncrement)} = true" : string.Empty);
                _content.AppendLine($"\t[{nameof(Column)}(\"{column.Name}\"{nullable}{autoIncrement})]");

                // Index
                var indexes = (
                    from i in column.Table.Indexes
                    where i.Columns.Count(x => x.Name == column.Name) > 0
                    select i);

                foreach (var index in indexes)
                {
                    string unique = (index.IsUnique ? $", {nameof(IndexAttribute.Unique)} = true" : string.Empty);
                    _content.AppendLine($"\t[{nameof(Index)}({nameof(IndexType)}.{index.Type}{unique})]");
                }

                //Foreign key
                var foreignKeys = (
                    from f in column.Table.ForeignKeys
                    where f.LocalColumn == column.Name
                    select f);

                foreach (var fkey in foreignKeys)
                {
                    string onUpdate = (fkey.OnUpdate == ReferentialAction.NoAction ? string.Empty : $", {nameof(ForeignKeyAttribute.OnUpdate)} = {nameof(ReferentialAction)}.{fkey.OnUpdate}");
                    string ondelete = (fkey.OnDelete == ReferentialAction.NoAction ? string.Empty : $", {nameof(ForeignKeyAttribute.OnDelete)} = {nameof(ReferentialAction)}.{fkey.OnDelete}");
                    _content.AppendLine($"\t[{nameof(ForeignKey)}(\"{fkey.ForeignTable}\", \"{fkey.ForeignColumn}\"{onUpdate}{ondelete})]");
                }
            }
        }

        private void Comment(Table table)
        {
            if (Settings.IgnoreComments) return;
            else
            {
                string summary = (string.IsNullOrEmpty(table.Comment)) ? $"Represents a {table.Name}." : table.Comment;
                _content.AppendLine("/// <summary>");
                _content.AppendLine($"/// {summary}");
                _content.AppendLine("/// </summary>");
            }
        }

        private void Comment(Column column)
        {
            if (Settings.IgnoreComments) return;
            else
            {
                string summary = (string.IsNullOrEmpty(column.Comment)) ? $"Gets or Sets the {column.Name}." : column.Comment;
                _content.AppendLine("\t/// <summary>");
                _content.AppendLine($"\t/// {summary}");
                _content.AppendLine("\t/// </summary>");
            }
        }

        #endregion Private Members
    }
}