namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct MySqlTemplateSettings
    {
        public static MySqlTemplateSettings Default = new MySqlTemplateSettings()
        {
            CommentsEnabled = true,
            DropTableIfExist = false,
            DropDataIfExist = false
        };

        public bool CommentsEnabled { get; set; }

        public bool DropDataIfExist { get; set; }

        public bool DropTableIfExist { get; set; }
    }
}