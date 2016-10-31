namespace Gigobyte.Daterpillar.TextTransformation
{
    public struct TemplateBuilderSettings
    {
        public static TemplateBuilderSettings Default = new TemplateBuilderSettings()
        {
            AppendScripts = true,
            AppendComments = true,
            TruncateDatabaseIfItExist = false
        };

        public bool AppendScripts { get; set; }

        public bool AppendComments { get; set; }

        public bool TruncateDatabaseIfItExist { get; set; }
    }
}