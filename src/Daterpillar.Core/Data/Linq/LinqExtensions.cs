namespace Gigobyte.Daterpillar.Data.Linq
{
    /// <summary>
    /// Provides extension methods for the <see cref="Linq"/> namespace.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Gets the SQL command to retrieve the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns>The SQL query.</returns>
        public static string ToSelectCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToSelectCommand(entity, style);
        }

        /// <summary>
        /// Gets the SQL command to insert the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns>The SQL command.</returns>
        public static string ToInsertCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToInsertCommand(entity, style);
        }

        /// <summary>
        /// Gets the SQL command to delete the specified <paramref name="entity"/> from a database.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="style">The style.</param>
        /// <returns>The SQL command.</returns>
        public static string ToDeleteCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToDeleteCommand(entity, style);
        }

        /// <summary>
        /// Escapes the specified <paramref name="value"/> into a well formatted SQL value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>The SQL command.</returns>
        public static string ToSQL(this object obj)
        {
            return SqlWriter.EscapeValue(obj);
        }
    }
}