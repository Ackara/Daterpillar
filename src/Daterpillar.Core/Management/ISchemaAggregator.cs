using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Management
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}