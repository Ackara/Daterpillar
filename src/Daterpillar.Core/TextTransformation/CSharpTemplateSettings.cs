namespace Gigobyte.Daterpillar.TextTransformation
{
    public class CSharpTemplateSettings
    {
        public static CSharpTemplateSettings Default = new CSharpTemplateSettings()
        {
            Namespace = Schema.Xmlns,

            AppendComments = true,
            AppendDataContracts = true,
            AppendSchemaInformation = true,
            AppendVirtualProperties = false
        };

        public string Namespace { get; set; }

        public bool AppendComments { get; set; }

        public bool AppendDataContracts { get; set; }

        public bool AppendSchemaInformation { get; set; }

        public bool AppendVirtualProperties { get; set; }
    }
}