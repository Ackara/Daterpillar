using System.Collections.Generic;

namespace Gigobyte.Daterpillar.Automation
{
    public class SchemaDiscrepancy
    {
        public Counter Counters;
        public Outcome Summary { get; set; }
        public IList<Discrepancy> Discrepancies { get; set; }

        public struct Counter
        {
            public int SourceTables;

            public int DestTables;
        }
    }
}