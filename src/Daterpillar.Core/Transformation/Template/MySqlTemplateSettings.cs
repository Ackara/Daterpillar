namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct MySqlTemplateSettings
    {
        public static MySqlTemplateSettings Default = new MySqlTemplateSettings()
        {
            AddScript = true,
            CommentsEnabled = true,
            DropDatabaseIfExist = false
        };

        public bool AddScript { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DropDatabaseIfExist { get; set; }
    }
}