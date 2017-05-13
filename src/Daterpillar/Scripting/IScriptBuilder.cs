﻿namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Define methods used for building scripts for a specific language.
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
        IScriptBuilder AppendLine(string text);

        /// <summary>
        /// Appends a copy of the specified string to this instance.
        /// </summary>
        /// <param name="text">The string to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(string text);

        /// <summary>
        /// Appends the string representation of a <see cref="Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(Schema schema);

        /// <summary>
        /// Appends the command to remove a <see cref="Schema" /> to this instance.
        /// </summary>
        /// <param name="schema">The schema to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Remove(Schema schema);

        /// <summary>
        /// Appends the string representation of a <see cref="Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(Table table);

        /// <summary>
        /// Appends the command to remove a <see cref="Table" /> to this instance.
        /// </summary>
        /// <param name="table">The table to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Remove(Table table);

        /// <summary>
        /// Appends the string representation of a <see cref="Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(Column column);

        /// <summary>
        /// Appends the command to remove a <see cref="Column" /> to this instance.
        /// </summary>
        /// <param name="column">The column to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Remove(Column column);

        /// <summary>
        /// Appends the string representation of a <see cref="Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(Index index);

        /// <summary>
        /// Appends the command to remove a <see cref="Index" /> to this instance.
        /// </summary>
        /// <param name="index">The index to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Remove(Index index);

        /// <summary>
        /// Appends the string representation of a <see cref="ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to append.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Append(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to remove a <see cref="ForeignKey" /> to this instance.
        /// </summary>
        /// <param name="foreignKey">The foreign key to remove.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Remove(ForeignKey foreignKey);

        /// <summary>
        /// Appends the command to update a <see cref="Table" /> to this instance.
        /// </summary>
        /// <param name="oldTable">The old table.</param>
        /// <param name="newTable">The new table.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Update(Table oldTable, Table newTable);

        /// <summary>
        /// Appends the command to update a <see cref="Column" /> to this instance.
        /// </summary>
        /// <param name="oldColumn">The old column.</param>
        /// <param name="newColumn">The new column.</param>
        /// <returns>A reference to this instance after the append operation has completed.</returns>
        IScriptBuilder Update(Column oldColumn, Column newColumn);

        /// <summary>
        /// Converts the value of this instance to a <see cref="System.String" />
        /// </summary>
        /// <returns>A string whose value is the same as this instance.</returns>
        string GetContent();

        /// <summary>
        /// Removes all characters from this instance.
        /// </summary>
        void Clear();
    }
}