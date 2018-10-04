using Acklann.Daterpillar.Configuration;

namespace Acklann.Daterpillar.Compilation
{
    public class Discrepancy<TSqlObject> where TSqlObject : ISQLObject
    {
        public Discrepancy(TSqlObject value)
        {
            Value = value;
        }

        public TSqlObject Value;

        public SqlAction Action { get; set; }
    }
}