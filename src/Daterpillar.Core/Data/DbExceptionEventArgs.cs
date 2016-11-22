namespace Acklann.Daterpillar.Data
{
    public sealed class DbExceptionEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbExceptionEventArgs"/> class.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public DbExceptionEventArgs(string command, string message, int errorCode)
        {
            Command = command;
            Message = message;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// The SQL command
        /// </summary>
        public readonly string Command;

        /// <summary>
        /// The message
        /// </summary>
        public readonly string Message;

        /// <summary>
        /// The error code
        /// </summary>
        public readonly int ErrorCode;
    }
}