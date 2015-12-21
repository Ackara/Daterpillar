namespace Ackara.Daterpillar.Data
{
    public struct ColumnInfo
    {
        /// <summary>
        /// Gets whether the column's primary key is auto incremented.
        /// </summary>
        public bool AutoIncremented { get; internal set; }

        /// <summary>
        /// Get whether the column is or one part of a primary key.
        /// </summary>
        public bool IsKey { get; internal set; }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the value assigned in the column.
        /// </summary>
        public object Value { get; internal set; }
    }
}