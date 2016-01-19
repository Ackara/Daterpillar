using System.Text;
using System.Text.RegularExpressions;

namespace Gigobyte.Daterpillar.Transformation.Template
{
    public class EnumTemplate
    {
        public string Transform(Enumeration enumeration)
        {
            var regex = new Regex(@"[^a-z0-9_]+", RegexOptions.IgnoreCase);

            var text = new StringBuilder();
            text.AppendLine($"public enum {enumeration.Name.ToPascalCase(' ', '_')}");
            text.AppendLine("{");

            foreach (var value in enumeration.Values)
            {
                string name = regex.Replace(value.Key, "_").ToPascalCase(' ', '_');
                text.AppendLine($"\t{name} = {value.Value},");
            }

            // remove last comma
            int lastComma = text.ToString().LastIndexOf(',');
            text.Remove(lastComma, 1);
            text.AppendLine("}");

            return text.ToString().Trim();
        }
    }
}