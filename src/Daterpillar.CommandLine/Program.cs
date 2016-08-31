using Gigobyte.Daterpillar.Arguments;
using Gigobyte.Daterpillar.Commands;
using System;

namespace Gigobyte.Daterpillar
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            InitializeWindow();

            start:
            var options = new Options();
            if (args.Length > 0)
            {
                CommandLine.Parser.Default.ParseArguments(args, options,
                    onVerbCommand: (verb, arg) =>
                    {
                        ICommand command = new CommandFactory().CrateInstance(verb);
                        try { _exitCode = command.Execute(arg); }
                        catch (Exception ex)
                        {
                            _exitCode = ExitCode.UnhandledException;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex);
                        }
                    });
            }
            else
            {
                Console.WriteLine(options.GetHelp());
                args = Console.ReadLine().Split(new char[] { ' ', '\t', '\n' });
                goto start;
            }
            Environment.Exit(_exitCode);
        }

        #region Private Members

        private static int _exitCode;

        private static void InitializeWindow()
        {
            Console.Title = $"{nameof(Daterpillar)} CLI";
        }

        #endregion Private Members
    }
}