namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct TSqlTemplateSettings
    {
        public static TSqlTemplateSettings Default = new TSqlTemplateSettings()
        {
            CommentsEnabled = true,
            DropSchema = false,
            DropTable = true
        };

        public bool CommentsEnabled { get; set; }
        public bool DropSchema { get; set; }
        public bool DropTable { get; set; }
    }
}