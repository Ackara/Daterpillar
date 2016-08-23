namespace Gigobyte.Daterpillar.TextTransformation
{
    public class NullTemplate : ITemplate
    {
        public string Transform(Schema schema)
        {
            return string.Empty;
        }
    }
}