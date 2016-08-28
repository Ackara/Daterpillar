using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Compare
{
    public interface ISchemaComparer : IComparer<Schema>
    {
        ChangeLog GetChanges(Schema source, Schema target);

        ChangeLog GetChanges(ISchemaAggregator source, ISchemaAggregator target);
    }
}