namespace Ackara.Daterpillar.Transformation.Template
{
    public struct SqlTemplateSettings
    {
        public static readonly SqlTemplateSettings Default = new SqlTemplateSettings()
        {
            Indent = true,
            ShowComments = true
        };

        public bool Indent { get; set; }

        public bool ShowComments { get; set; }
    }
}