using System.Data;

namespace Gigobyte.Daterpillar.Automation
{
    public interface ISchemaComparer : System.IDisposable
    {
        SchemaDiscrepancy Compare(ISchemaAggregator source, ISchemaAggregator target);
    }
}