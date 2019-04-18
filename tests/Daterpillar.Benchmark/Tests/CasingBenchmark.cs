using BenchmarkDotNet.Attributes;

namespace Acklann.Daterpillar.Tests
{
    public class CasingBenchmark
    {
        private static readonly string[] Input = new string[]
        {
            "Adipiscing elit proin risus", "lectus vestibulum quam sapien", "at", "est lacinia nisi venenatis", "ac enim in",
            "nunc proin at turpis", "diam", "neque duis bibendum", "cum sociis Natoque penatibus", "suscipitAfeugiat"
        };

        [Benchmark]
        public int PascalCase()
        {
            int length = 0;
            foreach (string i in Input)
                length += i.ToPascal().Length;

            return length;
        }

        [Benchmark]
        public int CamelCase()
        {
            int length = 0;
            foreach (string i in Input)
                length += i.ToCamel().Length;

            return length;
        }

        [Benchmark]
        public int SnakeCase()
        {
            int length = 0;
            foreach (string i in Input)
                length += i.ToSnake().Length;

            return length;
        }
    }
}