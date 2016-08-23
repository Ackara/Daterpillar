using CommandLine;

namespace Gigobyte.Daterpillar.Arguments
{
    public class Options
    {
        [VerbOption(MountVerb.Name, HelpText = "")]
        public MountVerb MountOption { get; set; }

        [VerbOption("sync", HelpText = "")]
        public SyncVerb SyncOption { get; set; }

        [HelpOption]
        public string GetHelp()
        {
            return CommandLine.Text.HelpText.AutoBuild(this);
        }
    }
}