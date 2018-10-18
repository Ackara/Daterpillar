namespace Acklann.Daterpillar.Exceptions
{
    internal class Error
    {
        public static string DuplicateSUID(string objectName, int suid, string kind = "table")
        {
            string msg = "Your {0} {2} SUID ({1}) is not unique.";

            return string.Format(msg, objectName, suid, kind);
        }

        public static string FileNotFound(string path, string kind = "file")
        {
            string msg = "Could not find {1} file at '{0}'.";

            return string.Format(msg, path, kind);
        }
    }
}