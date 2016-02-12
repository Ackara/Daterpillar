namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct SqlTemplateSettings
    {
        public static SqlTemplateSettings Default = new SqlTemplateSettings()
        {
            CommentEnabled = true,
            DropTableIfExist = false,
            DropDatabaseIfExist = true
        };

        public bool CommentEnabled { get; set; }

        public bool DropTableIfExist { get; set; }

        public bool DropDatabaseIfExist { get; set; }
    }
}