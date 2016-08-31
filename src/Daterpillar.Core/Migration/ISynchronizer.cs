using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;

namespace Gigobyte.Daterpillar.Migration
{
    public interface ISynchronizer
    {
        ChangeLog GenerateScript(Schema source, Schema target);

        ChangeLog GenerateScript(ISchemaAggregator source, ISchemaAggregator target);
    }
}