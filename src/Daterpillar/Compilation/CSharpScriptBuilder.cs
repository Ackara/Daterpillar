using Acklann.Daterpillar.Compilation.Resolvers;
using Acklann.Daterpillar.Configuration;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Compilation
{
    /// <summary>
    /// Provides functions used for building C# scripts.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Compilation.IScriptBuilder" />
    public class CSharpScriptBuilder : IScriptBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScriptBuilder" /> class.
        /// </summary>
        public CSharpScriptBuilder() : this(new CSharpScriptBuilderSettings()
        {
            AddConstants = true,
            IgnoreComments = false,
            AddSchemaAttributes = true,
            UseVirtualProperties = true,
            InheritEntityBaseClass = true,
            AddDataContractAttributes = false
        }, new CSharpTypeResolver())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScriptBuilder" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public CSharpScriptBuilder(CSharpScriptBuilderSettings settings) : this(settings, new CSharpTypeResolver())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpScriptBuilder" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The resolver.</param>
        public CSharpScriptBuilder(CSharpScriptBuilderSettings settings, ITypeResolver resolver)
        {
            _content = new StringBuilder();
            _typeResolver = resolver;
            Settings = settings;
        }

        /// <summary>
        /// The settings
        /// </summary>
        public CSharpScriptBuilderSettings Settings;

        /// <summary>
        /// Gets the number of characters within the script.
        /// </summary>
        /// <value>The length of the script.</value>
        public int Length
        {
            get { return _content.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return _content.Length == 0; }
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(string text)
        {
            _content.Append(text);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder AppendLine(string text)
        {
            _content.AppendLine(text);
            return this;
        }

        /// <summary>
        /// Appends the C# representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Schema schema)
        {
            foreach (var table in schema.Tables) Append(table);
            return this;
        }

        /// <summary>
        /// Appends the C# representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Table table)
        {
            Comment(table);
            AppendAttributes(table);
            _content.AppendLine($"public partial class {table.Name.ToPascalCase()}");
            _content.AppendLine("{");
            AppendConstants(table);
            foreach (var column in table.Columns) Append(column);
            _content.AppendLine("}");
            _content.AppendLine();
            return this;
        }

        /// <summary>
        /// Appends the C# representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Column column)
        {
            string name = column.Name.ToPascalCase();
            string type = _typeResolver.GetTypeName(column.DataType);
            string virtualKeyword = (Settings.UseVirtualProperties ? " virtual " : " ");

            Comment(column);
            AppendAttributes(column);
            _content.AppendLine($"\tpublic{virtualKeyword}{type} {name} {{ get; set; }}");
            _content.AppendLine();
            return this;
        }

        /// <summary>
        /// Appends the C# representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Index index)
        {
            return this;
        }

        /// <summary>
        /// Appends the C# representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(ForeignKey foreignKey)
        {
            return this;
        }

        /// <summary>
        /// Appends the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="valuePairs">The value pairs.</param>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        /// <param name="comment">The comment.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(string name, IDictionary<string, int> valuePairs, bool flag = false, string comment = null)
        {
            Comment("<summary>");
            Comment(comment ?? $"Represents a {name}.");
            Comment("</summary>");
            if (flag) _content.AppendLine("[System.Flags]");
            _content.AppendLine($"public enum {name}");
            _content.AppendLine("{");
            foreach (var item in valuePairs)
            {
                Comment("<summary>", "\t");
                Comment(comment ?? $"A {item.Key}.", "\t");
                Comment("</summary>", "\t");
                _content.AppendLine($"\t{item.Key.ToPascalCase()} = {item.Value},");
                _content.AppendLine();
            }
            _content.Remove((_content.Length - 5), 3);
            _content.AppendLine("}");
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        public IScriptBuilder Remove(Schema schema)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        public IScriptBuilder Remove(Table table)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        public IScriptBuilder Remove(Column column)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        public IScriptBuilder Remove(Index index)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        public IScriptBuilder Remove(ForeignKey foreignKey)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        public IScriptBuilder Update(Table oldTable, Table newTable)
        {
            return this;
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        public IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            return this;
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="T:System.String" />
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public string GetContent()
        {
            return GetContent(string.Empty);
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>System.String.</returns>
        public string GetContent(string header)
        {
            return string.Concat(header, _content);
        }

        /// <summary>
        /// Removes all characters from this instance.
        /// </summary>
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
                _content.AppendLine();
                foreach (var column in table.Columns)
                {
                    _content.AppendLine($"\tpublic const string {column.Name.ToPascalCase()}Column = \"{column.Name}\";");
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
                string scale = (column.DataType.Scale == 0 ? string.Empty : $", {column.DataType.Scale}");
                string precision = (column.DataType.Precision == 0 ? string.Empty : $", {column.DataType.Precision}");
                string dataType = $", \"{column.DataType.Name}\"{scale}{precision}";
                string autoIncrement = (column.AutoIncrement ? $", {nameof(ColumnAttribute.AutoIncrement)} = true" : string.Empty);
                string nullable = (column.IsNullable ? $", {nameof(ColumnAttribute.Nullable)} = true" : string.Empty);
                string defaultValue = (column.DefaultValue == null ? string.Empty : $", {nameof(ColumnAttribute.DefaultValue)} = \"{column.DefaultValue}\"");
                _content.AppendLine($"\t[{nameof(Column)}(\"{column.Name}\"{dataType}{nullable}{autoIncrement}{defaultValue})]");

                // Index
                var indexes = from i in column.Table.Indecies
                              where i.Columns.Count(x => x.Name == column.Name) > 0
                              select i;

                foreach (var index in indexes)
                {
                    string unique = (index.IsUnique ? $", {nameof(IndexAttribute.Unique)} = true" : string.Empty);
                    _content.AppendLine($"\t[{nameof(Index)}({nameof(IndexType)}.{index.Type}{unique})]");
                }

                //Foreign key
                var foreignKeys = from key in column.Table.ForeignKeys
                                  where key.LocalColumn == column.Name
                                  select key;

                foreach (var constraint in foreignKeys)
                {
                    string onUpdate = (constraint.OnUpdate == ReferentialAction.NoAction ? string.Empty : $", {nameof(ForeignKeyAttribute.OnUpdate)} = {nameof(ReferentialAction)}.{constraint.OnUpdate}");
                    string onDelete = (constraint.OnDelete == ReferentialAction.NoAction ? string.Empty : $", {nameof(ForeignKeyAttribute.OnDelete)} = {nameof(ReferentialAction)}.{constraint.OnDelete}");
                    _content.AppendLine($"\t[{nameof(ForeignKey)}(\"{constraint.ForeignTable}\", \"{constraint.ForeignColumn}\"{onUpdate}{onDelete})]");
                }
            }
        }

        private void Comment(Table table)
        {
            if (Settings.IgnoreComments) return;
            else
            {
                _content.AppendLine("/// <summary>");
                _content.AppendLine($"/// {(string.IsNullOrEmpty(table.Comment) ? $"Represents a {table.Name}." : table.Comment)}");
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

        private void Comment(string text, string pre = "")
        {
            if (Settings.IgnoreComments) return;
            else
            {
                _content.AppendLine($"{pre}/// {text}");
            }
        }

        #endregion Private Members
    }
}