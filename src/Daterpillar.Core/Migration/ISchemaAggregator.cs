namespace Acklann.Daterpillar.Migration
{
    public interface ISchemaAggregator : System.IDisposable
    {
        Schema FetchSchema();
    }
}