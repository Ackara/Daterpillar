namespace Gigobyte.Daterpillar.TextTransformation
{
    public class CSharpTemplateSettings
    {
        public static CSharpTemplateSettings Default = new CSharpTemplateSettings()
        {
            Namespace = Schema.Xmlns,

            CommentsEnabled = true,
            DataContractsEnabled = true,
            AppendSchemaInformation = true,
            VirtualPropertiesEnabled = false
        };

        public string Namespace { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DataContractsEnabled { get; set; }

        public bool AppendSchemaInformation { get; set; }

        public bool VirtualPropertiesEnabled { get; set; }
    }
}