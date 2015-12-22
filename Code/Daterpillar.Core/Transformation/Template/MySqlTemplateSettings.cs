namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct MySqlTemplateSettings
    {
        public static MySqlTemplateSettings Default = new MySqlTemplateSettings()
        {
            CommentsEnabled = true,
            DropSchemaAtBegining = false
        };

        public bool CommentsEnabled { get; set; }

        public bool DropSchemaAtBegining { get; set; }
    }
}