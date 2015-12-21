namespace Ackara.Daterpillar.Transformation.Template
{
    public struct CsharpTemplateSettings
    {
        public static CsharpTemplateSettings Default = new CsharpTemplateSettings()
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