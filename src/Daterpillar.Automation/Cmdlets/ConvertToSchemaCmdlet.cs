using Acklann.Daterpillar.Migration;
using System.Data;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that creates a new <see cref="Schema"/> instance from a file or <see cref="IDbConnection"/> . This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsData.ConvertTo, "Schema")]
    public sealed class ConvertToSchemaCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the input object.
        /// </summary>
        /// <value>The input object.</value>
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public object InputObject { get; set; }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (InputObject is PSObject ps) { InputObject = ps.BaseObject; }

            if (InputObject is string path)
            {
                WriteObject(Schema.Load(File.OpenRead(path)));
            }
            else if (InputObject is FileInfo file)
            {
                WriteObject(Schema.Load(file.OpenRead()));
            }
            else if (InputObject is IDbConnection connection)
            {
                using (IInformationSchema informationSchema = InformationSchemaFactory.CreateInstance(connection))
                {
                    WriteObject(informationSchema.FetchSchema());
                }
            }
            else { WriteWarning($"unable to process an object of type [{InputObject.GetType().Name}]."); }
        }
    }
}