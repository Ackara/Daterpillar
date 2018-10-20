namespace Acklann.Daterpillar
{
    public static class Error
    {
        public static string FileNotFound(string path, string kind = null)
        {
            string msg = "Could not find{1}file at '{0}'.";

            return string.Format(msg, path, (string.IsNullOrEmpty(kind) ? " " : $" {kind} "));
        }

        public static string NotWellFormedError(string path, string errorMsg)
        {
            string msg = "The following file contain error(s) '{0}'.\r\n{1}";

            return string.Format(msg, path, errorMsg);
        }
    }
}