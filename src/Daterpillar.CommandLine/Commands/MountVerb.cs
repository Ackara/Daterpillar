using CommandLine;

namespace Gigobyte.Daterpillar.Commands
{
    public sealed class MountVerb
    {
        public const string Name = "mount";

        [Option('s', "schema", Required = true, HelpText = "The path of the 'xddl.xml' schema.")]
        public string Path { get; set; }

        [Option('c', "connection", Required = true, HelpText = "The data store connection string.")]
        public string ConnectionString { get; set; }

        [Option('o', "override", HelpText = "Drop all DB objects if the database already exist.")]
        public bool Override { get; set; }

        [Option('p', "platform", DefaultValue = SupportedDatabase.MSSQL, HelpText = "The database platform in which to target.")]
        public SupportedDatabase Platform { get; set; }
    }
}