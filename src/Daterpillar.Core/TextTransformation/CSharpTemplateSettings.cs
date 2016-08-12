namespace Gigobyte.Daterpillar.TextTransformation
{
    public class CSharpTemplateSettings
    {
        public static CSharpTemplateSettings Default = new CSharpTemplateSettings()
        {
            Namespace = Schema.Xmlns,

            CommentsEnabled = true,
            DataContractsEnabled = true,
            SchemaAttributesEnabled = true,
            VirtualPropertiesEnabled = true
        };

        public string Namespace { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DataContractsEnabled { get; set; }

        public bool SchemaAttributesEnabled { get; set; }

        public bool VirtualPropertiesEnabled { get; set; }
    }
}