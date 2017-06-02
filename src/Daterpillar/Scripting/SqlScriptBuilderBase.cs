using Acklann.Daterpillar.TypeResolvers;
using System;
using System.Linq;
using System.Text;

namespace Acklann.Daterpillar.Scripting
{
    /// <summary>
    /// Provides basic functionality used for building scripts for a specific SQL language.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Scripting.IScriptBuilder" />
    public abstract class SqlScriptBuilderBase : IScriptBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlScriptBuilderBase" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="resolver">The type resolver.</param>
        public SqlScriptBuilderBase(SqlScriptBuilderSettings settings, ITypeResolver resolver)
        {
            Settings = settings;
            TypeResolver = resolver;
            _script = new StringBuilder();
        }

        /// <summary>
        /// The <see cref="SqlScriptBuilderSettings"/> object used to format the text.
        /// </summary>
        public SqlScriptBuilderSettings Settings;

        /// <summary>
        /// Gets the number of characters within the script.
        /// </summary>
        /// <value>The length of the script.</value>
        public int Length
        {
            get { return _script.Length; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty
        {
            get { return (_script.Length == 0); }
        }

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(string text)
        {
            _script.Append(text);
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder AppendLine(string text)
        {
            _script.AppendLine(text);
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Append(Schema schema);

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Append(Table table);

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Append(Column column);

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Append(Index index);

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Append(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Remove(Schema schema);

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Remove(Table table);

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Remove(Column column);

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Remove(Index index);

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Remove(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Update(Column oldColumn, Column newColumn);

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public abstract IScriptBuilder Update(Table oldTable, Table newTable);

        /// <summary>
        /// Converts the value of this instance to a <see cref="T:System.String" />
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public virtual string GetContent()
        {
            return GetContent(GetHeader());
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="T:System.String" />
        /// </summary>
        /// <param name="header">The header.</param>
        /// <returns>A string whose value is the same as this instance.</returns>
        public virtual string GetContent(string header)
        {
            return string.Concat(Environment.ExpandEnvironmentVariables(header), _script.ToString());
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return GetContent();
        }

        /// <summary>
        /// Removes all characters from this instance.
        /// </summary>
        public void Clear()
        {
            _script.Clear();
        }

        #region Protected Members

        /// <summary>
        /// The <see cref="StringBuilder"/> object used for storing and editing the script contents.
        /// </summary>
        protected StringBuilder _script;

        /// <summary>
        /// The <see cref="ITypeResolver"/> object used for mapping a http://static.acklann.com/schema/v2/daterpillar.xsd TypeName to another language's type name.
        /// </summary>
        protected ITypeResolver TypeResolver;

        /// <summary>
        /// Appends the string representation of a <see cref="Acklann.Daterpillar.Script" /> to this instance.
        /// </summary>
        /// <param name="script">The script object.</param>
        protected void Append(Script script)
        {
            if (Settings.IgnoreComments == false && string.IsNullOrWhiteSpace(script.Name) == false)
            {
                string line = string.Join(string.Empty, Enumerable.Repeat("-", 40));
                _script.AppendLine($"-- {line}");
                _script.AppendLine($"-- SCRIPT: {script.Name}");
                _script.AppendLine($"-- {line}");
            }

            _script.AppendLine(script.Content);
        }

        #endregion Protected Members

        #region Private Members

        internal void WriteComment(Table table)
        {
            if (Settings.IgnoreComments == false)
            {
                string line = string.Join(string.Empty, Enumerable.Repeat<string>("-", 40));

                _script.AppendLine($"-- {line}");
                _script.AppendLine($"-- TABLE [{table.Name}]");
                _script.AppendLine($"-- {line}");
            }
        }

        private string GetHeader()
        {
            if (Settings.ShowHeader)
            {
                string line = string.Join(string.Empty, Enumerable.Repeat("-", 40));

                var header = new StringBuilder();
                header.AppendLine($"-- {line}");
                header.AppendLine($"-- GENERATED BY: {Environment.GetEnvironmentVariable("USERNAME")}");
                header.AppendLine($"-- CREATED ON:   {DateTime.Now.ToString("MMMM dd, yyyy")}");
                header.AppendLine($"-- {line}");
                header.AppendLine();
                header.AppendLine();

                return header.ToString();
            }
            else return string.Empty;
        }

        #endregion Private Members
    }
}