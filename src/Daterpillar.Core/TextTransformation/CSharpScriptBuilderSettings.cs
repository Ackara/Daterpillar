namespace Gigobyte.Daterpillar.TextTransformation
{
    public class CSharpScriptBuilderSettings
    {
        public static CSharpScriptBuilderSettings Default = new CSharpScriptBuilderSettings()
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