using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar;
using Telerik.JustMock;
using System.Data;

namespace Tests.Daterpillar.UnitTest
{
    [TestClass]
    public class SchemaAggregatorFactoryTest
    {
        [TestMethod]
        [Owner(Test.Dev.Ackara)]
        public void CreateInstance_should_instaniate_a_schema_aggregator_object_whehn_the_partial_match_flag_is_enabled()
        {
            var sut = new SchemaAggregatorFactory();
            var type = sut.CreateInstance(nameof(SupportedDatabase.MSSQL), Mock.Create<IDbConnection>(), partialMatch: true);
            Assert.IsInstanceOfType(type, typeof(ISchemaAggregator));
        }
    }
}
