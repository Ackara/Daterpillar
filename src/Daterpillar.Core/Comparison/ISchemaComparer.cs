using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;

namespace Gigobyte.Daterpillar.Compare
{
    public interface ISchemaComparer
    {
        ChangeLog GetChanges(Schema source, Schema target);

        ChangeLog GetChanges(ISchemaAggregator source, ISchemaAggregator target);
    }
}