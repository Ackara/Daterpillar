using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Compare
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}