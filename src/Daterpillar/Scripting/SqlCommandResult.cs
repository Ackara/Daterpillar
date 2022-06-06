namespace Acklann.Daterpillar.Scripting
{
    public readonly struct SqlCommandResult
    {
        public SqlCommandResult(int code, string message)
            : this(code == 0, code, 0, message, null)
        {
        }

        public SqlCommandResult(string command)
            : this(true, 0, 0, null, command) { }

        public SqlCommandResult(string command, int changes)
            : this(true, 0, changes, null, command) { }

        public SqlCommandResult(string command, int errorCode, string message)
            : this(false, errorCode, 0, message, command) { }

        public SqlCommandResult(bool success, int errorCode, long changes, string message, string command)
        {
            Success = success;
            Changes = changes;
            ErrorCode = errorCode;
            ErrorMessage = message;
            Changes = changes;
            Command = command;
        }

        public bool Success { get; }

        public long Changes { get; }

        public int ErrorCode { get; }

        public string ErrorMessage { get; }

        public string Command { get; }

        #region Operator

        public static implicit operator bool(SqlCommandResult obj)
        {
            return obj.Success;
        }

        #endregion Operator
    }
}