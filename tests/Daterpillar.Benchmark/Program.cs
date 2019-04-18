using Acklann.VBench;
using Benchmarkable;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;

namespace Acklann.Daterpillar
{
    internal class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            Debug();
#else
            Release();
#endif
        }

        private static void Debug()
        {
            Console.WriteLine("running ...");Console.WriteLine();

            var obj = new Foo();
            var dic = new Dictionary<string, object>
            {
                { nameof(Foo.Id), 0 },
                { nameof(Foo.Name), 0 }
            };
            dynamic d = new System.Dynamic.ExpandoObject();

            void baseLine() { obj.Id = 10; obj.Name = "sally"; }
            void dictionary() { dic[nameof(Foo.Id)] = 10; dic[nameof(Foo.Name)] = "sally"; }
            void dynamicM() { d.Id = 10; d.Name = "sally"; }


            
            
            var test = new Benchmark();
            test.Add(baseLine, nameof(baseLine));
            test.Add(dictionary, nameof(dictionary));
            test.Add(dynamicM, nameof(dynamicM));
            test.Run().Print();
            

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

        private class Foo
        { public int Id; public string Name; }
    }
}