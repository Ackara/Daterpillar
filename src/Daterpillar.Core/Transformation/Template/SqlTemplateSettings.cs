namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct SqlTemplateSettings
    {
        public static SqlTemplateSettings Default = new SqlTemplateSettings()
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