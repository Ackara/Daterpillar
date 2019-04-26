using Acklann.Daterpillar.Linq;
using BenchmarkDotNet.Attributes;
using Dapper;
using System.Data;
using System.Linq;

namespace Acklann.Daterpillar.Tests
{
    public class QueryBenchmark
    {
        private static readonly string _query = $"select * from {nameof(User)};";

        [Benchmark]
        public int Daterpillar()
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

        [Benchmark]
        public int Dapper()
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