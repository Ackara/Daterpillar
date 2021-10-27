namespace Acklann.Daterpillar.Scripting
{
    public readonly struct SqlCommandResult
    {
        public SqlCommandResult(int code, string message)
            : this(code == 0, code, 0, message)
        {
        }

        public SqlCommandResult(bool success, int errorCode, long changes, string message)
        {
            Success = success;
            Changes = changes;
            ErrorCode = errorCode;
            ErrorMessage = message;
            Changes = changes;
        }

        public bool Success { get; }

        public long Changes { get; }

        public int ErrorCode { get; }

        public string ErrorMessage { get; }

        #region Operator

        public static implicit operator bool(SqlCommandResult obj)
        {
            return obj.Success;
        }

        #endregion Operator
    }
}