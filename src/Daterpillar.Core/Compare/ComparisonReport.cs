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

        public ReportConclusions Summary { get; set; }

        public Counter Source { get; set; }

        public Counter Target { get; set; }

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
            var conclusion = ReportConclusions.None;
            if (Source.TotalObjects == 0)
                conclusion |= ReportConclusions.SourceEmpty;

            if (Target.TotalObjects == 0)
                conclusion |= ReportConclusions.TargetEmpty;

            if (Discrepancies.Count > 0)
                conclusion |= ReportConclusions.NotEqual;

            if (Discrepancies.Count == 0)
                conclusion |= ReportConclusions.Equal;
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