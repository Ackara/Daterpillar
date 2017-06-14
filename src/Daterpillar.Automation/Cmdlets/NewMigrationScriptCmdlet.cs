using Acklann.Daterpillar.Migration;
using Acklann.Daterpillar.Scripting;
using System;
using System.Linq;
using System.Management.Automation;

namespace Acklann.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that creates a SQL migration script. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsCommon.New, "MigrationScript")]
    public sealed class NewMigrationScriptCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the source <see cref="Schema"/>.
        /// </summary>
        /// <value>The source.</value>
        [Alias("left", "src", "from")]
        [Parameter(Position = 1)]
        public Schema Source { get; set; }

        /// <summary>
        /// Gets or sets the target <see cref="Schema"/>.
        /// </summary>
        /// <value>The target.</value>
        [Alias("right", "dest", "to")]
        [Parameter(Position = 2, ValueFromPipeline = true)]
        public Schema Target { get; set; }

        /// <summary>
        /// Gets or sets the script's syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Parameter(Position = 3)]
        [Alias("type", "syn", "ext", "extension")]
        public string Syntax { get; set; } = nameof(Daterpillar.Syntax.MSSQL);

        /// <summary>
        /// Gets or sets the <see cref="IScriptBuilder"/> instance that will be used to generate the script.
        /// </summary>
        /// <value>The <see cref="IScriptBuilder"/> instance.</value>
        [Parameter(Position = 4)]
        [Alias("builder", "b")]
        public IScriptBuilder ScriptBuilder { get; set; }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            if (ScriptBuilder == null)
            {
                ScriptBuilder = ScriptBuilderFactory.CreateInstance(Syntax);
                WriteVerbose($"using an {nameof(IScriptBuilder)} of type [{ScriptBuilder.GetType().Name}].");
            }

            if (ScriptBuilder is NullScriptBuilder) { WriteWarning($"'{Syntax}' is not a valid value, try using one of the following instead {string.Join(", ", Enum.GetNames(typeof(Syntax)).Skip(1))}"); }
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            MigrationState state = _comparer.Compare(Source, Target, ScriptBuilder, out string modifications);
            WriteObject(new PSObject(new
            {
                State = state,
                Modifications = modifications,
                Script = ScriptBuilder.GetContent()
            }));
        }

        #region Private Members

        private SchemaComparer _comparer = new SchemaComparer();

        #endregion Private Members
    }
}