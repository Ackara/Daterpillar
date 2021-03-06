using Acklann.Daterpillar.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    [RankColumn]
    public class QueryBenchmark
    {
        private static readonly string _query = $"select * from {nameof(User)};";

        [Benchmark(Description = nameof(Daterpillar))]
        public int RunDaterpillar()
        {
            int count = 0;
            using (IDbConnection connection = Helper.CreateDatabase())
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                var results = connection.Select<User>(_query).ToArray();
                count = results.Length;
            }

            return count;
        }

        [Benchmark(Description = nameof(Dapper))]
        public int RunDapper()
        {
            int count = 0;
            using (IDbConnection connection = Helper.CreateDatabase())
            {
                if (connection.State != ConnectionState.Open) connection.Open();
                var results = connection.Query(_query).ToArray();
                count = results.Length;
            }

            return count;
        }
    }
}