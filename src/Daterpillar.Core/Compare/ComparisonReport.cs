using System.Collections;
using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Compare
{
    public class ComparisonReport : IEnumerable<Discrepancy>
    {
        public ComparisonReport()
        {
            Discrepancies = new List<Discrepancy>();
        }

        public Counter Source;
        public Counter Target;

        public ComparisionReportConclusions Summary { get; set; }

        public int TotalSourceTables { get; set; }

        public int TotalTargetTables { get; set; }

        public IList<Discrepancy> Discrepancies { get; set; }

        public IEnumerator<Discrepancy> GetEnumerator()
        {
            return Discrepancies.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Summarize()
        {
            var conclusion = ComparisionReportConclusions.None;
            if (Source.TotalObjects == 0)
                conclusion |= ComparisionReportConclusions.SourceEmpty;

            if (Target.TotalObjects == 0)
                conclusion |= ComparisionReportConclusions.TargetEmpty;

            if (Discrepancies.Count > 0)
                conclusion |= ComparisionReportConclusions.NotEqual;

            if (Discrepancies.Count == 0)
                conclusion |= ComparisionReportConclusions.Equal;

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