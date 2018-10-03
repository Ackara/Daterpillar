using Acklann.Daterpillar.Commands;
using Acklann.NShellit;
using Acklann.NShellit.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Acklann.Daterpillar
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            IEnumerable<Type> commandList = (from t in typeof(ICommand).Assembly.DefinedTypes
                                             where t.IsAbstract == false && t.IsInterface == false && typeof(ICommand).IsAssignableFrom(t)
                                             select t);

            var parser = new Parser();
            int exitCode = parser.Map<ICommand, int>(args, commandList,
                (cmd) => cmd.Execute(),
                (err) => ExitCode.ParsingError);
#if DEBUG
            Console.WriteLine($"exit-code: {exitCode}");
            Console.WriteLine("press any key to exit...");
            Console.ReadKey();
#endif
            return exitCode;
        }
    }
}