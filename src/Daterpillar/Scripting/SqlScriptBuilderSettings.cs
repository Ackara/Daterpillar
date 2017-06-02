namespace Acklann.Daterpillar.Scripting
{
    /// <summary>
    /// Specifies a set of features to support on the <see cref="IScriptBuilder" /> object when the <see cref="IScriptBuilder.GetContent" /> method is called.
    /// </summary>
    public class SqlScriptBuilderSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether a header should be appended before the script. The default is true.
        /// </summary>
        /// <value><c>true</c> if [show header]; otherwise, <c>false</c>.</value>
        public bool ShowHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Script"/> objects should be removed. The default is false.
        /// </summary>
        /// <value><c>true</c> if <see cref="Script"/> objects should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreScripts { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the SQL objects comments should be removed. The default is false.
        /// </summary>
        /// <value><c>true</c> if all comments should be ignored; otherwise, <c>false</c>.</value>
        public bool IgnoreComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the 'CREATE DATABASE' command should be appended. The default is false.
        /// </summary>
        /// <value><c>true</c> if the 'CREATE DATABASE' command should be appended; otherwise, <c>false</c>.</value>
        public bool AppendCreateSchemaCommand { get; set; }
    }
}