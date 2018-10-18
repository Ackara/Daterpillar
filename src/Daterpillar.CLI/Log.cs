using System;

namespace Acklann.Daterpillar
{
    internal static class Log
    {
        public static int CouldNotFind(string file, string context = "")
        {
            string msg = string.Format("[ERROR] Could not find{1}file at '{0}'.", file, $" {context} ");

            PrintError(msg);
            return 404;
        }

        public static int NotWellFormedError(string path, string errorMsg)
        {
            PrintError(string.Format("The following file contain error(s) '{0}'.", path));
            foreach (string line in errorMsg.Split("\r\n", StringSplitOptions.RemoveEmptyEntries))
            {
                PrintError(line);
            }

            return 400;
        }

        public static void Print(string msg, ConsoleColor color = ConsoleColor.DarkGray)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(string.Format("{1}{0}", msg, "\r\n"));
            Console.ResetColor();
        }

        public static void PrintError(string msg, ConsoleColor color = ConsoleColor.Red)
        {
            Console.ForegroundColor = color;
            Console.Error.WriteLine(string.Format("{1}{0}", msg, "\r\n"));
            Console.ResetColor();
        }
    }
}