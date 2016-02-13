namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct SqlTemplateSettings
    {
        public static SqlTemplateSettings Default = new SqlTemplateSettings()
        {
            RunScript = true,
            CommentEnabled = false,
            DropDatabaseIfExist = true
        };

        public bool RunScript { get; set; }

        public bool CommentEnabled { get; set; }

        public bool DropDatabaseIfExist { get; set; }
    }
}