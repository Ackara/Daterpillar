namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class SqlTemplateSettings
    {
        public static SqlTemplateSettings Default = new SqlTemplateSettings()
        {
            AddScript = true,
            UseDatabase = false,
            CreateSchema = false,
            CommentsEnabled = true,
            DropDatabaseIfExist = false
        };

        public bool AddScript { get; set; }

        public bool UseDatabase { get; set; }
        public bool CreateSchema { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DropDatabaseIfExist { get; set; }

    }
}