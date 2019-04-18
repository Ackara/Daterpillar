using Acklann.VBench;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Acklann.Daterpillar
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            QuickTest();
#else
            Release(args);
#endif
        }

        private static void QuickTest()
        {
            Console.WriteLine("running ..."); Console.WriteLine();

            const string sample1 = "the dog bark woof woof";
            const string sample2 = "the dog bark woof woof";
            void plusequal()
            {
                string c = sample1 + sample2;
            }

            void builder()
            {
                var r = new StringBuilder();
                r.Append(sample1);
                r.Append(sample2);
            }

            void concat()
            {
                string c = string.Concat(sample1, sample2);
            }

#if DEBUG
            var list = new Action[] { plusequal, builder, concat };

            var regex = new Regex(@"(?i)[^a-z]");
            var test = new Benchmarkable.Benchmark();
            for (int i = 0; i < list.Length; i++)
                test.Add(list[i], regex.Replace(list[i].Method.Name.Replace($"<{nameof(QuickTest)}>g", string.Empty), string.Empty));
            test.Run().Print();
#endif
            // ==================== EXIT ==================== //
            Console.WriteLine();
            Console.Write("press any key to exit ...");
            Console.ReadKey();
        }

        private static void Release(string[] args)
        {
            BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, DefaultConfig.Instance
                .With(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default)
                .With(new TimelineExporter())
                );
        }

        private class Foo { public int Id; public string Name; }
    }
}