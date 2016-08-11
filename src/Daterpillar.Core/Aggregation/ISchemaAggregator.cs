using Gigobyte.Daterpillar.TextTransformation;

namespace Gigobyte.Daterpillar.Aggregation
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}