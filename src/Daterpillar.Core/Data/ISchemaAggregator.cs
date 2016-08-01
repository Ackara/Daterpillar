using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Data
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema(string connectionString);
    }
}