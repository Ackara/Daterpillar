using System.Data;

namespace Gigobyte.Daterpillar.Management
{
    public interface ISchemaComparer : System.IDisposable
    {
        SchemaDiscrepancy Compare(ISchemaAggregator source, ISchemaAggregator target);
    }
}