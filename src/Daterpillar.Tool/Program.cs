using Acklann.Daterpillar.Tool.Commands;
using CommandLine;

namespace Acklann.Daterpillar.Tool
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // docs: https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools-how-to-create

            Parser.Default.ParseArguments<ExportCommand, GenerateCommand, NewSchemaCommand>(args)
                .WithParsed<NewSchemaCommand>((o) => o.Execute())
                .WithParsed<ExportCommand>((o) => o.Execute())
                .WithParsed<GenerateCommand>((o) => o.Execute());
        }
    }
}