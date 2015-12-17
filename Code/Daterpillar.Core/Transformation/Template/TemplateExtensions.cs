using System.Text;

namespace Ackara.Daterpillar.Transformation.Template
{
    public static class TemplateExtensions
    {
        public static string ToTitle(this string word)
        {
            throw new System.NotImplementedException();
        }

        public static string ToPascal(this string word)
        {

            throw new System.NotImplementedException();
        }



        internal static void TrimComma(this StringBuilder builder)
        {
            int commaIndex = builder.ToString().LastIndexOf(',');
            builder.Remove(commaIndex, 1);
        }

        internal static string ToText(this ForeignKeyRule rule)
        {
            string output;

            switch (rule)
            {
                default:
                case ForeignKeyRule.NoAction:
                    output = "NO ACTION";
                    break;

                case ForeignKeyRule.Cascade:
                    output = "CASCADE";
                    break;

                case ForeignKeyRule.SetNull:
                    output = "SET NULL";
                    break;

                case ForeignKeyRule.SetDefault:
                    output = "SET DEFAULT";
                    break;

                case ForeignKeyRule.Restrict:
                    output = "RESTRICT";
                    break;
            }

            return output;
        }
    }
}