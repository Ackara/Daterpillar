using Gigobyte.Daterpillar.Transformation;

namespace Gigobyte.Daterpillar.Compare
{
    public class SchemaComparer : ISchemaComparer
    {
        public ComparisonReport GenerateReport(ISchemaAggregator source, ISchemaAggregator target)
        {
            using (source)
            {
                using (target)
                {
                    return GenerateReport(source.FetchSchema(), target.FetchSchema());
                }
            }
        }

        public ComparisonReport GenerateReport(Schema source, Schema target)
        {
            _report = new ComparisonReport();

            return _report;
        }

        public int Compare(Schema source, Schema target)
        {
            var report = GenerateReport(source, target);
            
            throw new System.NotImplementedException();
        }

        #region Private Members

        private ComparisonReport _report;

        #endregion Private Members
    }
}