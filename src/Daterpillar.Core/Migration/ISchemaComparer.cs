namespace Gigobyte.Daterpillar.Migration
{
    public interface ISchemaComparer
    {
        ComparisonReport GetChanges(Schema source, Schema target);

        ComparisonReport GetChanges(ISchemaAggregator source, ISchemaAggregator target);
    }
}