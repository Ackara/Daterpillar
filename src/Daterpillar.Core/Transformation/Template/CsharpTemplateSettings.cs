namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class CSharpTemplateSettings
    {
        public static CSharpTemplateSettings Default = new CSharpTemplateSettings()
        {
            Namespace = Schema.Xmlns,

            CommentsEnabled = true,
            DataContractsEnabled = true,
            SchemaAnnotationsEnabled = true,
            VirtualPropertiesEnabled = true
        };

        public string Namespace { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DataContractsEnabled { get; set; }

        public bool SchemaAnnotationsEnabled { get; set; }

        public bool VirtualPropertiesEnabled { get; set; }
    }
}