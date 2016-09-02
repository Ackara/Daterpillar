using Gigobyte.Daterpillar.Aggregation;

namespace Gigobyte.Daterpillar.Migration
{
    public interface ISynchronizer
    {
        byte[] GenerateScript(Schema source, Schema target);

        byte[] GenerateScript(ISchemaAggregator source, ISchemaAggregator target);
    }
}