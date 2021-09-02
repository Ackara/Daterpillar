using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsCommon.New, "ConnectionString")]
    public class NewConnectionStringCmdlet : Cmdlet
    {
        [Alias("connStr")]
        [Parameter(ValueFromPipeline = true, Mandatory = true)]
        public string ConnectionString { get; set; }

        protected override void ProcessRecord()
        {
            

            base.WriteObject(new ConnectionInfo());
        }
    }
}