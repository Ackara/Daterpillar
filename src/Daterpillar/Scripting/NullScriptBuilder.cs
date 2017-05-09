namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Represents a null <see cref="IScriptBuilder"/>.
    /// </summary>
    /// <seealso cref="Ackara.Daterpillar.Scripting.IScriptBuilder" />
    public class NullScriptBuilder : IScriptBuilder
    {
        /// <summary>
        /// Gets the number of characters within the script.
        /// </summary>
        /// <value>The length of the script.</value>
        public int Length => 0;

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        public bool IsEmpty => true;

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(string text)
        {
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Schema schema)
        {
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Table table)
        {
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Column column)
        {
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(Index index)
        {
            return this;
        }

        /// <summary>
        /// Appends the string representation of a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Append(ForeignKey foreignKey)
        {
            return this;
        }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder AppendLine(string text)
        {
            return this;
        }

        /// <summary>
        /// Removes all characters from this instance.
        /// </summary>
        public void Clear()
        {
        }

        /// <summary>
        /// Converts the value of this instance to a <see cref="T:System.String" />
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        public string GetContent()
        {
            return null;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Remove(Schema schema)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Remove(Table table)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Remove(Column column)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Remove(Index index)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to remove a <see cref="T:Ackara.Daterpillar.ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Remove(ForeignKey foreignKey)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Update(Table oldTable, Table newTable)
        {
            return this;
        }

        /// <summary>
        /// Appends the command to update a <see cref="T:Ackara.Daterpillar.Column" /> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        public IScriptBuilder Update(Column oldColumn, Column newColumn)
        {
            return this;
        }
    }
}