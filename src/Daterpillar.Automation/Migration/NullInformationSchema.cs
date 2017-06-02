namespace Acklann.Daterpillar.Migration
{
    /// <summary>
    /// Represents a null <see cref="IInformationSchema" />.
    /// </summary>
    /// <seealso cref="Acklann.Daterpillar.Migration.IInformationSchema" />
    public sealed class NullInformationSchema : IInformationSchema
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Creates a <see cref="T:Ackara.Daterpillar.Schema" /> instance.
        /// </summary>
        /// <param name="name">The name of the schema.</param>
        /// <returns>A <see cref="T:Ackara.Daterpillar.Schema" /> instance.</returns>
        public Schema FetchSchema(string name)
        {
            return new Schema();
        }

        /// <summary>
        /// Creates a <see cref="T:Ackara.Daterpillar.Schema" /> instance.
        /// </summary>
        /// <returns>A <see cref="T:Ackara.Daterpillar.Schema" /> instance.</returns>
        public Schema FetchSchema()
        {
            return new Schema();
        }
    }
}