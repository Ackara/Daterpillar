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

            int exitCode = 0;
            var options = new Options();
            
            if (args.Length > 0)
            {
                var errorOccurred = CommandLine.Parser.Default.ParseArguments(args, options,
                    onVerbCommand: (verb, arg) =>
                    {
                        ICommand command = new CommandFactory().CrateInstance(verb);
                        try { exitCode = command.Execute(arg); }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(ex);
                            exitCode = 1;
                        }
                    });
            }
            else Console.WriteLine(options.GetHelp());

            Environment.Exit(exitCode);
        }

        #region Private Members

        private static readonly string _logo = @"
  _____        _                  _ _ _
 |  __ \      | |                (_) | |
 | |  | | __ _| |_ ___ _ __ _ __  _| | | __ _ _ __
 | |  | |/ _` | __/ _ \ '__| '_ \| | | |/ _` | '__|
 | |__| | (_| | ||  __/ |  | |_) | | | | (_| | |
 |_____/ \__,_|\__\___|_|  | .__/|_|_|_|\__,_|_|
                           | |
                           |_|
";

        private static void InitializeWindow()
        {
            Console.Title = $"{nameof(Daterpillar)} CLI";
            Console.WriteLine(_logo.Trim());
        }

        #endregion Private Members
    }
}