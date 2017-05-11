using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Ackara.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "Script")]
    public class ConvertToScriptCmdlet : Cmdlet
    {
        public string Path { get; set; }

        public Schema InputObject { get; set; }

        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }
    }
}
