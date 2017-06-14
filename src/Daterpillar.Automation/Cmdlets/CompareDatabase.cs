using Acklann.Daterpillar.Migration;
using Acklann.Daterpillar.Scripting;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsData.Compare, "Database")]
    public class CompareDatabase : Cmdlet
    {
        [Alias("left", "src")]
        [Parameter(Mandatory = true, Position = 1)]
        public Schema Source { get; set; }

        [Alias("right", "dest")]
        [Parameter(Mandatory = true, Position = 2)]
        public Schema Target { get; set; }

        protected override void ProcessRecord()
        {
            var state = new SchemaComparer().Compare(Source, Target, new NullScriptBuilder(), out string modifications);
            WriteObject(new PSObject(new
            {
                State = state,
                Differences = modifications
            }));
        }
    }
}