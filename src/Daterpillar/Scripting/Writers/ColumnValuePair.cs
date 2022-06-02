namespace Acklann.Daterpillar.Scripting.Writers
{
    public struct ColumnValuePair
    {
        public ColumnValuePair(string column, object value)
        {
            ColumnName = column;
            Value = value;
        }

        public string ColumnName { get; set; }

        public object Value { get; set; }
    }
}