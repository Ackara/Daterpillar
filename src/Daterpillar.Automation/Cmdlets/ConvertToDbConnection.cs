using Acklann.Daterpillar.Migration;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "DbConnection")]
    public class ConvertToDbConnection : Cmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("type", "syn", "ext", "extension")]
        [ValidateSet(nameof(Daterpillar.Syntax.MSSQL), nameof(Daterpillar.Syntax.MySQL), nameof(Daterpillar.Syntax.SQLite))]
        public string Syntax { get; set; }

        [Parameter(Mandatory = true)]
        [Alias("conn", "connStr")]
        public string ConnectionString { get; set; }

        protected override void ProcessRecord()
        {
            using (var factory = new ServerManagerFactory().Create(Syntax, ConnectionString))
            {
                WriteObject(factory.GetConnection());
            }
        }
    }
}