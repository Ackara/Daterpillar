﻿using Ackara.Daterpillar.Scripting;
using System;
using System.Linq;
using System.Management.Automation;

namespace Ackara.Daterpillar.Cmdlets
{
    /// <summary>
    /// A powershell cmdlet that generates a script from a <see cref="Schema"/> instance. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.Management.Automation.Cmdlet" />
    [Cmdlet(VerbsData.ConvertTo, "Script")]
    public sealed class ConvertToScriptCmdlet : Cmdlet
    {
        /// <summary>
        /// Gets or sets the <see cref="Schema"/> Instance.
        /// </summary>
        /// <value>The input object.</value>
        [Alias("src", "schema")]
        [Parameter(Position = 0, ValueFromPipeline = true)]
        public Schema InputObject { get; set; }

        /// <summary>
        /// Gets or sets the script's syntax.
        /// </summary>
        /// <value>The syntax.</value>
        [Parameter(Position = 1)]
        [Alias("s", "syn", "ext", "extension")]
        public string Syntax { get; set; } = nameof(Daterpillar.Syntax.MSSQL);

        /// <summary>
        /// Gets or sets the <see cref="IScriptBuilder"/> instance that will be used to generate the script.
        /// </summary>
        /// <value>The <see cref="IScriptBuilder"/> instance.</value>
        [Parameter]
        [Alias("builder", "b")]
        public IScriptBuilder ScriptBuilder { get; set; }

        /// <summary>
        /// Begins the processing.
        /// </summary>
        protected override void BeginProcessing()
        {
            if (ScriptBuilder == null)
            {
                ScriptBuilder = ScriptBuilderFactory.CreateInstance(Syntax.ToLower());
                WriteVerbose($"using an {nameof(IScriptBuilder)} of type [{ScriptBuilder.GetType().Name}].");
            }

            if (ScriptBuilder is NullScriptBuilder) { WriteWarning($"'{Syntax}' is not a valid value, try using one of the following instead ({string.Join(", ", Enum.GetNames(typeof(Syntax)).Skip(1))})."); }
        }

        /// <summary>
        /// Processes the record.
        /// </summary>
        protected override void ProcessRecord()
        {
            if (InputObject != null)
            {
                ScriptBuilder.Append(InputObject);
                WriteObject(ScriptBuilder.GetContent());
            }
        }
    }
}