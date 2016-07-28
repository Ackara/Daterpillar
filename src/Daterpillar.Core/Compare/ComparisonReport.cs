using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gigobyte.Daterpillar.Compare
{
    public class ComparisonReport : IEnumerable<Discrepancy>
    {
        public ComparisonReport()
        {
            Discrepancies = new List<Discrepancy>();
        }

        public Outcomes Summary { get; set; }

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
            if (Discrepancies.Count == 0)
            {
                Summary = Outcomes.Equal;
            }
        }
    }
}