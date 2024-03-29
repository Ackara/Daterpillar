namespace Acklann.Daterpillar.Scripting
{
    public readonly struct SqlQueryResult
    {
        public SqlQueryResult(object data, string message = default)
        {
            Data = data;
            ErrorMessage = message;
        }

        public object Data { get; }

        public string ErrorMessage { get; }

        public bool Success
        {
            get => Data != null && Data != default;
        }

        #region Operator

        public static implicit operator bool(SqlQueryResult obj) => obj.Success;

        #endregion Operator
    }

    public readonly struct QueryResult<T>
    {
        public QueryResult(T data, string message = default)
        {
            Data = data;
            ErrorMessage = message;
        }

        public T Data { get; }

        public string ErrorMessage { get; }

        public bool Success
        {
            get => Data != null;
        }

        #region Operator

        public static implicit operator T(QueryResult<T> obj)
        {
            return obj.Data;
        }

        public static implicit operator bool(QueryResult<T> obj) => obj.Success;

        public static implicit operator SqlQueryResult(QueryResult<T> obj) => new SqlQueryResult(obj.Data, obj.ErrorMessage);

        #endregion Operator
    }
}