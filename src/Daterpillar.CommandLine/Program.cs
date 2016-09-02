using Gigobyte.Daterpillar.Commands;
using System;

namespace Gigobyte.Daterpillar
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            InitializeWindow();

            do
            {
                if (args.Length > 0)
                {
                    CommandLine.Parser.Default.ParseArguments(args, _commandLineOptions,
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
                            finally { Console.ResetColor(); }
                        }); break;
                }
                else
                {
                    Console.WriteLine(_commandLineOptions.GetHelp());
                    Console.Write("> ");
                    args = Console.ReadLine().Split(new char[] { ' ', '\t', '\n' });
                }
            } while (true);
            Environment.Exit(_exitCode);
        }

        #region Private Members

        private static int _exitCode;
        private static Options _commandLineOptions;

        private static void InitializeWindow()
        {
            Console.Title = $"{nameof(Daterpillar)} CLI";
        }

        #endregion Private Members
    }
}