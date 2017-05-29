namespace Ackara.Daterpillar.Scripting
{
    /// <summary>
    /// Specifies a set of features to support on the <see cref="CSharpScriptBuilder" /> object when the <see cref="IScriptBuilder.GetContent" /> method is called.
    /// </summary>
    public class CSharpScriptBuilderSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the xml comments should not be rendered. The default is false.
        /// </summary>
        /// <value><c>true</c> if comment should not be rendered; otherwise, <c>false</c>.</value>
        public bool IgnoreComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the class should be decorated with <see cref="Ackara.Daterpillar" /> attributes. The default is true.
        /// </summary>
        /// <value><c>true</c> if schema attributes should be appended; otherwise, <c>false</c>.</value>
        public bool AddSchemaAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use virtual properties. The default is true.
        /// </summary>
        /// <value><c>true</c> if use virtual properties; otherwise, <c>false</c>.</value>
        public bool UseVirtualProperties { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Ackara.Daterpillar"/> class should be inherited. The default is true.
        /// </summary>
        /// <value><c>true</c> if the <see cref="Ackara.Daterpillar"/> should be inherited; otherwise, <c>false</c>.</value>
        public bool InheritEntityBaseClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the class should be decorated with DataContract attributes. The default is false.
        /// </summary>
        /// <value><c>true</c> if DataContract attributes should be added; otherwise, <c>false</c>.</value>
        public bool AddDataContractAttributes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [add constants]. The default is true.
        /// </summary>
        /// <value><c>true</c> if [add constants]; otherwise, <c>false</c>.</value>
        public bool AddConstants { get; set; }
    }
}