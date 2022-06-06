using CommandLine;
using Daterpillar.Tool.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daterpillar.Tool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // docs: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create

            Parser.Default.ParseArguments<ExportCommand, GenerateCommand>(args)
                .WithParsed<ExportCommand>((x) => x.Execute())
                .WithParsed<GenerateCommand>((x) => x.Execute());
        }
    }
}