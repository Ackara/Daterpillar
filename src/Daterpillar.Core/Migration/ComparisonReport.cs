namespace Gigobyte.Daterpillar.Migration
{
    public class ComparisonReport
    {
        public Counter Source;

        public Counter Target;

        public int Discrepancies { get; set; }

        public ComparisonReportConclusions Summary { get; set; }

        public void Summarize()
        {
            var conclusion = ComparisonReportConclusions.None;
            if (Source.TotalObjects == 0)
                conclusion |= ComparisonReportConclusions.SourceEmpty;

            if (Target.TotalObjects == 0)
                conclusion |= ComparisonReportConclusions.TargetEmpty;

            if (Discrepancies > 0)
                conclusion |= ComparisonReportConclusions.NotEqual;

            if (Discrepancies == 0)
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