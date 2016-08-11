using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar.TextTransformation;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Compare
{
    public interface ISchemaComparer : IComparer<Schema>
    {
        ComparisonReport GenerateReport(Schema source, Schema target);

        ComparisonReport GenerateReport(ISchemaAggregator source, ISchemaAggregator target);
    }
}