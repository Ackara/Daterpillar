namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class MySQLTemplateSettings
    {
        public static MySQLTemplateSettings Default = new MySQLTemplateSettings()
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