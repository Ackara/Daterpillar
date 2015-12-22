using System.Text;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class EnumTemplate
    {
        public string Transform(Enumeration enumeration)
        {
            var text = new StringBuilder();
            text.AppendLine($"public enum {enumeration.Name.ToPascalCase(' ', '_')}");
            text.AppendLine("{");

            foreach (var value in enumeration.Values)
            {
                text.AppendLine($"\t{value.Key.ToPascalCase(' ', '_')} = {value.Value},");
            }

            // remove last comma
            int lastComma = text.ToString().LastIndexOf(',');
            text.Remove(lastComma, 1);
            text.AppendLine("}");

            return text.ToString().Trim();
        }
    }
}