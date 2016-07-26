using Gigobyte.Daterpillar.Transformation;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Compare
{
    public interface ISchemaComparer : System.IDisposable
    {
        ComparisonReport Compare(Schema source, Schema target);

        ComparisonReport Compare(ISchemaAggregator source, ISchemaAggregator target);
    }
}