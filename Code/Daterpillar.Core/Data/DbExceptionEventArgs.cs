namespace Gigobyte.Daterpillar.Data
{
    public sealed class DbExceptionEventArgs : System.EventArgs
    {
        public DbExceptionEventArgs(string command, string message, int errorCode)
        {
            Command = command;
            Message = message;
            ErrorCode = errorCode;
        }

        public readonly string Command;

        public readonly string Message;

        public readonly int ErrorCode;
    }
}