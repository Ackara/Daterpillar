namespace Ackara.Daterpillar.Migration
{
    /// <summary>
    /// Defines methods to create a <see cref="Schema"/> instance using a SQL server "information schema" table.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IInformationSchema : System.IDisposable
    {
        /// <summary>
        /// Creates a <see cref="Schema"/> instance using the information found for the specified schema.
        /// </summary>
        /// <returns>A <see cref="Schema"/> instance.</returns>
        Schema FetchSchema();

        /// <summary>
        /// Creates a <see cref="Schema"/> instance using the information found for the specified schema.
        /// </summary>
        /// <param name="name">The name of the schema.</param>
        /// <returns>A <see cref="Schema"/> instance.</returns>
        Schema FetchSchema(string name);
    }
}