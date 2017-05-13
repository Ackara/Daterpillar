using Ackara.Daterpillar.Scripting;
using System.IO;
using System.Management.Automation;

namespace Ackara.Daterpillar.Cmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "Script")]
    public class ConvertToScriptCmdlet : Cmdlet
    {
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public object InputObject { get; set; }

        [Parameter]
        public Syntax Syntax { get; set; } = Syntax.MSSQL;
        
        protected override void ProcessRecord()
        {
            IScriptBuilder script = new ScriptBuilderFactory().Create(Syntax);
            
            if (InputObject is Schema schema)
            {
                script.Append(schema);
                WriteObject(script.GetContent());
            }
            else if (InputObject is string path)
            {
                script.Append(Schema.Load(File.OpenRead(path)));
                WriteObject(script.GetContent());
            }
            else if (InputObject is PSObject obj)
            {
                if (obj.BaseObject is FileInfo file)
                {
                    script.Append(Schema.Load(file.OpenRead()));
                    WriteObject(script.GetContent());
                }
            }
        }
    }
}