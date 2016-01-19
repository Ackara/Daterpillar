namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct SQLiteTemplateSettings
    {
        public static SQLiteTemplateSettings Default = new SQLiteTemplateSettings()
        {
            CommentsEnabled = true,
            DropTable = true
        };

        public bool CommentsEnabled { get; set; }

        public bool DropTable { get; set; }
    }
}