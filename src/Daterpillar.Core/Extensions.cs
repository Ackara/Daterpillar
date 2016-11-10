namespace Gigobyte.Daterpillar
{
    /// <summary>
    /// Provide extension methods for the <see cref="Daterpillar" /> namespace.
    /// </summary>
    public static class Extensions
    {
        public static string ToText(this ForeignKeyRule rule)
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

        public static ForeignKeyRule ToEnum(this string value)
        {
            value = value.Replace(' ', '_').ToUpper();
            return (ForeignKeyRule)System.Enum.Parse(typeof(ForeignKeyRule), value);
        }

        public static IndexType ToIndexType(this string value)
        {
            throw new System.NotImplementedException();
        }

        internal static string GetName(this Index index, int count = 1)
        {
            if (string.IsNullOrEmpty(index.Name))
            {
                return $"{index.TableRef.Name}_idx{count}".ToLower();
            }
            else
            {
                return index.Name;
            }
        }

        internal static string GetName(this ForeignKey constriant, int count = 1)
        {
            if (string.IsNullOrEmpty(constriant.Name))
            {
                return $"{constriant.LocalColumn.ToLower()}_TO_{constriant.ForeignTable.ToLower()}_{constriant.ForeignColumn.ToLower()}_fkey{count}";
            }
            else
            {
                return constriant.Name;
            }
        }
    }
}