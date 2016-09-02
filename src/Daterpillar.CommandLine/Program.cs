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

            do
            {
                var commandLineOptions = new Options();
                if (args.Length > 0)
                {
                    CommandLine.Parser.Default.ParseArguments(args, commandLineOptions,
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
                    Console.WriteLine(commandLineOptions.GetHelp());
                    args = Console.ReadLine().Split(new char[] { ' ', '\t', '\n' });
                }
            } while (true);
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