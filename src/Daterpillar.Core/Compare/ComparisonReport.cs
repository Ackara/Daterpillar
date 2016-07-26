using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Gigobyte.Daterpillar.Compare
{
    [DataContract]
    public class ComparisonReport : IEnumerable<Discrepancy>
    {
        public Counter Counters;

        [DataMember]
        public Outcome Summary { get; set; }

        [DataMember]
        public IList<Discrepancy> Discrepancies { get; set; }

        public IEnumerator<Discrepancy> GetEnumerator()
        {
            foreach (var item in Discrepancies) { yield return item; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Counter
        {
            public int SourceTables;

            public int DestTables;
        }
    }
}