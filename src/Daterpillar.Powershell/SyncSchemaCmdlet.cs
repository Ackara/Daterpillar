using System.Management.Automation;

namespace Acklann.Daterpillar
{
    [Cmdlet(VerbsData.Sync, "Schema", ConfirmImpact = ConfirmImpact.Medium, SupportsShouldProcess = true)]
    public class SyncSchemaCmdlet : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string MigrationsDirectory { get; set; }

        [Parameter()]
        public string ConnectionString { get; set; }

        protected override void BeginProcessing()
        {
            
        }

        protected override void ProcessRecord()
        {
            
        }

    }
}