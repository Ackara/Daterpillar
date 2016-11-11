namespace Gigobyte.Daterpillar.Migration
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}