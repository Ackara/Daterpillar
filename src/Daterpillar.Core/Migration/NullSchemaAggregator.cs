namespace Acklann.Daterpillar.Migration
{
    public sealed class NullSchemaAggregator : ISchemaAggregator
    {
        public void Dispose()
        {
        }

        public Schema FetchSchema()
        {
            return new Schema();
        }
    }
}