namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Define methods used for building a script for a specific language.
    /// </summary>
    public interface IScriptBuilder
    {
        /// <summary>
        /// Gets the number of characters within the script.
        /// </summary>
        /// <value>The length of the script.</value>
        int Length { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is empty.
        /// </summary>
        /// <value><c>true</c> if this instance is empty; otherwise, <c>false</c>.</value>
        bool IsEmpty { get; }

        /// <summary>
        /// Appends a copy of the specified string followed by the default line terminator to the end of this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        string AppendLine(string text);

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        string Append(string text);

        /// <summary>
        /// Appends the string representation of a <see cref="Schema"/> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>System.String.</returns>
        string Append(Schema schema);

        /// <summary>
        /// Appends the command to remove a <see cref="Schema"/> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>System.String.</returns>
        string Remove(Schema schema);

        /// <summary>
        /// Appends the string representation of a <see cref="Table"/> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>System.String.</returns>
        string Append(Table table);

        /// <summary>
        /// Appends the command to remove a <see cref="Table"/> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>System.String.</returns>
        string Remove(Table table);

        /// <summary>
        /// Appends the string representation of a <see cref="Column"/> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>System.String.</returns>
        string Append(Column column);

        /// <summary>
        /// Appends the command to remove a <see cref="Column"/> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>System.String.</returns>
        string Remove(Column column);

        /// <summary>
        /// Appends the string representation of a <see cref="Index"/> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>System.String.</returns>
        string Append(Index index);

        /// <summary>
        /// Appends the command to remove a <see cref="Index"/> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>System.String.</returns>
        string Remove(Index index);

        /// <summary>
        /// Appends the string representation of a <see cref="ForeignKey"/> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>System.String.</returns>
        string Append(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to remove a <see cref="ForeignKey"/> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>System.String.</returns>
        string Remove(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to update a <see cref="Table"/> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>System.String.</returns>
        string UpdateTable(Table oldTable, Table newTable);

        /// <summary>
        /// Appends the command to update a <see cref="Column"/> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>System.String.</returns>
        string UpdateColumn(Column oldColumn, Column newColumn);

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String"/>
        /// </summary>
        /// <returns>A script.</returns>
        string GetContent();

        /// <summary>
        /// Removes all characters from this instance.
        /// </summary>
        void Clear();
    }
}