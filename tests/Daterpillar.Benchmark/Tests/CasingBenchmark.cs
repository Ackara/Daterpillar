using BenchmarkDotNet.Attributes;

namespace Acklann.Daterpillar.Tests
{
    public class CasingBenchmark
    {
        private static readonly string[] Input = new string[]
        {
            "quis", "augue vestibulum ante ipsum", "faucibus", "nec molestie", "pede ullamcorper augue a", "et commodo vulputate justo in",
            "enim lorem ipsum dolor sit", "montes nascetur ridiculus mus", "orci", "metus", "porttitor id consequat in", "odio", "lorem vitae mattis nibh ligula", "quis augue",
            "dapibus nulla suscipit", "aenean lectus pellentesque eget", "blandit ultrices enim lorem ipsum", "quis", "vel lectus in", "neque sapien placerat ante nulla", "integer ac", "vel",
            "a nibh in quis", "phasellus", "ut", "aliquet maecenas leo", "justo in hac habitasse platea", "dui nec", "tellus nulla ut", "molestie sed justo", "potenti nullam porttitor lacus at",
            "et tempus semper est", "ridiculus mus vivamus vestibulum sagittis", "quam a", "iaculis congue", "gravida sem praesent id massa",
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