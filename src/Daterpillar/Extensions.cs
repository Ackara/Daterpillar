namespace Ackara.Daterpillar
{
    public static class Extensions
    {
        internal static string ToText(this Order order)
        {
            string output;

            switch (order)
            {
                default:
                case Order.Ascending:
                    output = "ASC";
                    break;

                case Order.Descending:
                    output = "DESC";
                    break;
            }

            return output;
        }

        internal static string ToText(this ReferentialAction action)
        {
            string output;

            switch (action)
            {
                default:
                case ReferentialAction.NoAction:
                    output = "NO ACTION";
                    break;

                case ReferentialAction.Cascade:
                    output = "CASCADE";
                    break;

                case ReferentialAction.SetNull:
                    output = "SET NULL";
                    break;

                case ReferentialAction.SetDefault:
                    output = "SET DEFAULT";
                    break;

                case ReferentialAction.Restrict:
                    output = "RESTRICT";
                    break;
            }

            return output;
        }
    }
}