namespace Acklann.Daterpillar.Scripting.Writers
{
    public struct ColumnValuePair
    {
        public ColumnValuePair(string column, object value)
        {
            MemberName = "";
            ColumnName = column;
            Value = value;
        }

        public string MemberName { get; set; }

        public string ColumnName { get; set; }

        public object Value { get; set; }
    }
}