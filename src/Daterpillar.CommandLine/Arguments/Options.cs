using CommandLine;

namespace Gigobyte.Daterpillar.Arguments
{
    public class Options
    {
        [VerbOption(MountVerb.Name, HelpText = "")]
        public MountVerb MountOption { get; set; }

        [VerbOption("compare", HelpText = "")]
        public CompareVerb CompareOption { get; set; }

        [VerbOption("sync", HelpText = "")]
        public SyncVerb SyncOption { get; set; }

        [CommandLine.HelpOption]
        public string GetHelp()
        {
            return CommandLine.Text.HelpText.AutoBuild(this);
        }
    }
}