namespace Gigobyte.Daterpillar.Data.Linq
{
    /// <summary>
    /// Represent a SQL formatting style.
    /// </summary>
    public enum QueryStyle
    {
        /// <summary>
        /// Standard SQL
        /// </summary>
        SQL,

        /// <summary>
        /// The SQLite
        /// </summary>
        SQLite,

        /// <summary>
        /// MySQL
        /// </summary>
        MySQL,

        /// <summary>
        /// Microsoft's T-SQL
        /// </summary>
        TSQL
    }
}