namespace Gigobyte.Daterpillar.TextTransformation
{
    public struct ScriptBuilderSettings
    {
        public static ScriptBuilderSettings Default = new ScriptBuilderSettings()
        {
            AppendScripts = true,
            AppendComments = true,
            CreateDatabase = true,
            TruncateDatabaseIfItExist = false
        };

        public bool AppendScripts { get; set; }

        public bool AppendComments { get; set; }

        public bool CreateDatabase { get; set; }

        public bool TruncateDatabaseIfItExist { get; set; }
    }
}