using Acklann.VBench;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;

namespace Acklann.Daterpillar
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG

#endif
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, DefaultConfig.Instance
                .With(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default)
                .With(new TimelineExporter())
                );
        }
    }
}
