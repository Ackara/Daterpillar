namespace Gigobyte.Daterpillar.Transformation.Template
{
    public struct NotifyPropertyChangedTemplateSettings
    {
        public static NotifyPropertyChangedTemplateSettings Default = new NotifyPropertyChangedTemplateSettings()
        {
            Namespace = Schema.Xmlns,

            CommentsEnabled = true,
            DataContractsEnabled = true,
            SchemaAnnotationsEnabled = true,
            VirtualPropertiesEnabled = true,
            PartialRaisePropertyChangedMethodEnabled = false
        };

        public string Namespace { get; set; }

        public bool CommentsEnabled { get; set; }

        public bool DataContractsEnabled { get; set; }

        public bool SchemaAnnotationsEnabled { get; set; }

        public bool VirtualPropertiesEnabled { get; set; }

        public bool PartialRaisePropertyChangedMethodEnabled { get; set; }
    }
}