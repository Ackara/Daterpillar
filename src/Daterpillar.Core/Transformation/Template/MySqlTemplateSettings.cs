namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct MySqlTemplateSettings
    {
        public static MySqlTemplateSettings Default = new MySqlTemplateSettings()
        {
            CommentsEnabled = true,
            DropSchema = false
        };

        public bool CommentsEnabled { get; set; }

        public bool DropSchema { get; set; }
    }
}