namespace Gigobyte.Daterpillar.Data.Linq
{
    public static class LinqExtensions
    {
        public static string ToSelectCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToSelectCommand(entity, style);
        }

        public static string ToInsertCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToInsertCommand(entity, style);
        }

        public static string ToDeleteCommand(this EntityBase entity, QueryStyle style = QueryStyle.SQL)
        {
            return SqlWriter.ConvertToDeleteCommand(entity, style);
        }

        public static string ToSQL(this object obj)
        {
            return SqlWriter.EscapeValue(obj);
        }
    }
}