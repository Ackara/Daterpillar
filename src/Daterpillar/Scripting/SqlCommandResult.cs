namespace Acklann.Daterpillar.Scripting
{
    public readonly struct SqlCommandResult
    {
        public SqlCommandResult(int code, string message) : this(0, code, message)
        {
        }

        public SqlCommandResult(int changes, int errorCode, string message)
        {
            Changes = changes;
            ErrorCode = errorCode;
            ErrorMessage = message;
        }

        public bool Success
        {
            get => ErrorCode == 0;
        }

        public int Changes { get; }

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