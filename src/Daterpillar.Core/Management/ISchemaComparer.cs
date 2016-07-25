using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Management
{
    public interface ISchemaComparer : System.IDisposable
    {
        SchemaDiscrepancy Compare(Schema source, Schema target);

        SchemaDiscrepancy Compare(ISchemaAggregator source, ISchemaAggregator target);
    }
}