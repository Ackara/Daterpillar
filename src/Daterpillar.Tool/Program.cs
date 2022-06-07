using Acklann.Daterpillar.Commands;
using CommandLine;

namespace Acklann.Daterpillar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // docs: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create

            Parser.Default.ParseArguments<ExportCommand, GenerateCommand>(args)
                .WithParsed<ExportCommand>((o) => o.Execute())
                .WithParsed<GenerateCommand>((o) => o.Execute());
        }
    }
}