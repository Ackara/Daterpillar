namespace Acklann.Daterpillar.Scripting
{
    public readonly struct QueryResult<T>
    {
        public QueryResult(T data, string message = default)
        {
            Data = data;
            ErrorMessage = message;
        }

        public T Data { get; }

        public string ErrorMessage { get; }

        #region Operator

        public static implicit operator T(QueryResult<T> obj)
        {
            return obj.Data;
        }

        #endregion Operator
    }
}