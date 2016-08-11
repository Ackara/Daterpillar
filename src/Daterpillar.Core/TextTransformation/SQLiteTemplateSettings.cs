namespace Gigobyte.Daterpillar.TextTransformation
{
    public class SQLiteTemplateSettings
    {
        public static SQLiteTemplateSettings Default = new SQLiteTemplateSettings()
        {
            CommentsEnabled = true,
            DropTableIfExist = true
        };

        public bool CommentsEnabled { get; set; }

        public bool DropTableIfExist { get; set; }
    }
}