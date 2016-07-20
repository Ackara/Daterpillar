using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Automation
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}