using CommandLine;

namespace Gigobyte.Daterpillar.Commands
{
    public sealed class Options
    {
        

        [VerbOption(MountVerb.Name, HelpText = "Push schema to database.")]
        public MountVerb MountOption { get; set; }

        [VerbOption(SyncVerb.Name, HelpText = "Synchronize database schema and/or data.")]
        public SyncVerb SyncOption { get; set; }

        [HelpOption]
        public string GetHelp()
        {
            return CommandLine.Text.HelpText.AutoBuild(this);
        }
    }
}