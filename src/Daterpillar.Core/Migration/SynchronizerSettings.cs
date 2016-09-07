namespace Gigobyte.Daterpillar.Migration
{
    public struct SynchronizerSettings
    {
        public static SynchronizerSettings Default = new SynchronizerSettings()
        {
            IncludeComments = true,
            IncludeDropCommands = false
        };

        public bool IncludeComments { get; set; }

        public bool IncludeDropCommands { get; set; }
    }
}