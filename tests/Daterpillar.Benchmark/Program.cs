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

            const string sample = "the dog bark woof woof";
            void plusequal()
            {
                string r = ""; char c;
                for (int i = 0; i < sample.Length; i++)
                {
                    c = sample[i];
                    r += c;
                }
            }

            void builder()
            {
                var r = new StringBuilder(); char c;
                for (int i = 0; i < sample.Length; i++)
                {
                    c = sample[i];
                    r.Append(c);
                }

                r.ToString();
            }

            void array()
            {
                var r = new char[sample.Length]; char c;
                for (int i = 0; i < sample.Length; i++)
                {
                    c = sample[i];
                    r[i] = c;
                }
            }

            void span()
            {
                Span<char> r = new Span<char>();
                ReadOnlySpan<char> t = sample.AsSpan();

                for (int i = 0; i < t.Length; i++)
                {
                    r.Fill(t[i]);
                }

                r.ToString();
            }

            void spanBuilderB()
            {
                char c; ReadOnlySpan<char> t = sample.AsSpan();
                var r = new StringBuilder();
                for (int i = 0; i < t.Length; i++)
                {
                    c = t[i];
                    r.Append(c);
                }

                r.ToString();
            }

            void spanBuilderA()
            {
                char c; ReadOnlySpan<char> t = sample.AsSpan();
                var r = new StringBuilder();
                for (int i = 0; i < t.Length; i++)
                {
                    r.Append(t[i]);
                }

                r.ToString();
            }

#if DEBUG
            var list = new Action[] { array, plusequal, builder, spanBuilderB, spanBuilderA, span };

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