using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Gigobyte.Daterpillar.Aggregation;
using Gigobyte.Daterpillar;
using Telerik.JustMock;
using System.Data;
using Tests.Daterpillar.Constants;

namespace Tests.Daterpillar.Tests
{
    [TestClass]
    public class SchemaAggregatorFactoryTest
    {
        [TestMethod]
        [Owner(Dev.Ackara)]
        public void CreateInstance_should_instantiate_a_schema_aggregator_object_when_the_partial_match_flag_is_enabled()
        {
            var sut = new SchemaAggregatorFactory();
            var type = sut.CreateInstance(nameof(SupportedDatabase.MSSQL), Mock.Create<IDbConnection>(), partialMatch: true);
            Assert.IsInstanceOfType(type, typeof(ISchemaAggregator));
        }
    }
}
