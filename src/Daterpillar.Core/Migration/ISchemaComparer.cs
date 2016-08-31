using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;

namespace Gigobyte.Daterpillar.Migration
{
    public interface ISchemaComparer
    {
        ChangeLog GetChanges(Schema source, Schema target);

        ChangeLog GetChanges(ISchemaAggregator source, ISchemaAggregator target);
    }
}