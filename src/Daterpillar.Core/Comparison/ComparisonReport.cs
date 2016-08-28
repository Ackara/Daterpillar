using System.Collections;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Compare
{
    public class ComparisonReport : IEnumerable<Modification>
    {
        public ComparisonReport()
        {
            Discrepancies = new List<Modification>();
        }

        public Counter Source;

        public Counter Target;

        public ComparisonReportConclusions Summary { get; set; }

        public int TotalSourceTables { get; set; }

        public int TotalTargetTables { get; set; }

        public IList<Modification> Discrepancies { get; set; }

        public IEnumerator<Modification> GetEnumerator()
        {
            return Discrepancies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Summarize()
        {
            var conclusion = ComparisonReportConclusions.None;
            if (Source.TotalObjects == 0)
                conclusion |= ComparisonReportConclusions.SourceEmpty;

            if (Target.TotalObjects == 0)
                conclusion |= ComparisonReportConclusions.TargetEmpty;

            if (Discrepancies.Count > 0)
                conclusion |= ComparisonReportConclusions.NotEqual;

            if (Discrepancies.Count == 0)
                conclusion |= ComparisonReportConclusions.Equal;

            Summary = conclusion;
        }

        public struct Counter
        {
            public int TotalObjects
            {
                get { return TotalTables + TotalColumns + TotalIndexes + TotalForeignKeys; }
            }

            public int TotalTables { get; set; }

            public int TotalColumns { get; set; }

            public int TotalIndexes { get; set; }

            public int TotalForeignKeys { get; set; }
        }
    }
}