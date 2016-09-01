namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Provide extension methods for the <see cref="Daterpillar"/> namespace.
    /// </summary>
    public static class Extensions
    {
        internal static string ToText(this ForeignKeyRule rule)
        {
            string output;

            switch (rule)
            {
                default:
                case ForeignKeyRule.NONE:
                    output = "NO ACTION";
                    break;

                case ForeignKeyRule.CASCADE:
                    output = "CASCADE";
                    break;

                case ForeignKeyRule.SET_NULL:
                    output = "SET NULL";
                    break;

                case ForeignKeyRule.SET_DEFAULT:
                    output = "SET DEFAULT";
                    break;

                case ForeignKeyRule.RESTRICT:
                    output = "RESTRICT";
                    break;
            }

            return output;
        }
    }
}