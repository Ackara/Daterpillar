namespace Gigobyte.Daterpillar.Linq
{
    /// <summary>
    /// Represent a SQL formatting style.
    /// </summary>
    public enum QueryStyle
    {
        /// <summary>
        /// Standard SQL format
        /// </summary>
        SQL,

        /// <summary>
        /// SQLite format
        /// </summary>
        SQLite,

        /// <summary>
        /// MySQL format
        /// </summary>
        MySQL,

        /// <summary>
        /// Microsoft's T-SQL format
        /// </summary>
        TSQL
    }
}